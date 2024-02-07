using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections;
using UnityEngine.Rendering;

namespace M8 {

public sealed class SerialPortClient : MonoBehaviour
{
    #region Public properties
    // [SerializeField]
    // private MeshFilter screenMesh;
    [SerializeField]
    private RenderTexture screenTexture;
    [SerializeField]
    private Material screenMaterial;

    // [SerializeField]
    // private string portName;

    #endregion

    #region Serial communication

    private bool _hasReceivedDeviceInfo;
    private bool _isConnected;
    
    private SerialPort _port;
    private SlipParser _slip;
    private CommandParser _parser;
    private ScreenRenderer _renderer;
    private InputHandler _input;

    IEnumerator AttemptConnection(string portName)
    {
        const int maxWaitFrames = 3;
        _hasReceivedDeviceInfo = false;
        _isConnected = false;
        
        try
        {
            OpenPort(portName);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        for (int i = 0; i <= maxWaitFrames; ++i)
        {
            DoUpdate();
            if (_hasReceivedDeviceInfo)
            {
                _isConnected = true;
                break;
            }

            yield return null;
        }
    }
    
    void OpenPort(string portName)
    {
        SerialPortUtil.Configure(_port, portName);
        _port.Open();
        _port.Write(new [] {'E', 'R'}, 0, 2);
    }

    void OnMessageReceived(ReadOnlySpan<byte> message)
    {
        if (_parser.IsDrawCommand(message))
        {
            _renderer.Push(_parser.MakeDrawCommand(message));
            return;
        }

        if (_parser.IsDeviceInfo(message))
        {
            _parser.PrintDeviceInfo(message);
            _hasReceivedDeviceInfo = true;
            return;
        }

        // Unsupported message
    }

    void SendInput()
      => _port.Write(new [] {(byte)'C', _input.CurrentState}, 0, 2);

    #endregion

    void PrintPortNames()
    {
        foreach (var portName in SerialPort.GetPortNames())
        {
            Debug.Log(portName);
        }
    }

    #region MonoBehaviour implementation

    private IEnumerator Start()
    {
        _port = new SerialPort();
        _slip = new SlipParser();
        _parser = new CommandParser();
        _renderer = new ScreenRenderer(screenMaterial, screenTexture);
        _input = GetComponent<InputHandler>();
        //RenderPipelineManager.endFrameRendering += OnSRPCallback;

        _slip.OnReceived = OnMessageReceived;
        var ports = SerialPort.GetPortNames();

        foreach (var port in ports)
        {
            Debug.Log($"Attempting to connect to M8 on port [{port}]");
            yield return AttemptConnection(port);
            if (_isConnected)
            {
                Debug.Log($"Connection to port [{port}] succeeded!");
                break;
            }
            _port?.Close();
        }
        
        if (!_isConnected)
            Debug.LogError("Failed to find a valid port to use");
    }

    void OnDisable()
    { 
        _port?.Close();
        //RenderPipelineManager.endFrameRendering -= OnSRPCallback;
        
    }

    private void Update()
    {
        if (!_isConnected)
            return;
        
        DoUpdate();
        _renderer.DrawBuffered();
    }

    void DoUpdate()
    {
        while (_port.BytesToRead > 0) _slip.FeedByte(_port.ReadByte());
        if (_input.UpdateState()) SendInput();
    }

    private void OnPostRender()
      => _renderer.DrawBuffered();

    private void OnSRPCallback(ScriptableRenderContext _, Camera[] __)
        => _renderer.DrawBuffered();

    #endregion
}

} // namespace M8
