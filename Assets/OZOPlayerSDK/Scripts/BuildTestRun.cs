//Copyright © 2016 Nokia Corporation and/or its subsidiary(-ies). All rights reserved.

using UnityEngine;
using System.Collections;
using OZO;

/// <summary>
/// Test base class
/// </summary>
abstract public class Test
{
    protected string _name = "";
    protected bool _started = false;
    protected bool _ended = false;

    protected bool _success = false;
    protected string _errorMessage = "";

    protected static string TEST_BASE = "OZOUnityPluginTest::";

    public bool isStarted
    {
        get
        {
            return _started;
        }
    }
    public bool isEnded
    {
        get
        {
            return _ended;
        }
    }

    //static
    public static bool isRunning
    {
        get
        {
            return _running;
        }
    }
    protected static bool _running = false;
    public static void BeginTests()
    {
        Debug.Log(TEST_BASE + "BEGIN\n");
        _running = true;
    }
    public static void EndTests()
    {
        Debug.Log(TEST_BASE  + "FINISH\n");
        _running = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Begin()
    {
        Debug.Log(TEST_BASE + _name + ": START\n");
        _started = true;
        _ended = false;
        _success = false;
    }

public void Result()
    {
        Debug.Log(TEST_BASE + _name + ((_success)? ": OK" : ": ERROR: " + _errorMessage) + "\n");
        if(!_success)
        {
            EndTests();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        _ended = true;
    }

    abstract public void Start();
    abstract public void End();
}

/// <summary>
/// Test the SDK Initialization
/// </summary>
public class TestInit : Test
{
    OZOPlayer _player;

    public TestInit(OZOPlayer player)
    {
        _name = "TestInit";
        _player = player;
    }

    override public void Start()
    {
        Begin();

        if (_player != null)
        {
            IOZOPlayer play = _player;
            Debug.Log(TEST_BASE + _name + ": SDK version: " + play.GetVersion() + "\n");

            if (null != play)
            {
                _success = true;
            }
            else
            {
                _success = false;
                _errorMessage = "Cannot Initialize the native SDK";
            }
        }
        else
        {
            _errorMessage = "No Player Set in Unity Scene";
        }
    }
    override public void End()
    {
        Result();
    }
}

/// <summary>
/// Test Video Playback
/// </summary>
public class TestPlay : Test
{
    private IOZOPlayer _play;
    private string _videoFileName;

    public TestPlay(OZOPlayer player, string videoFileName)
    {
        _videoFileName = videoFileName;
        _play = player;
        _name = "TestPlay";
    }
    override public void Start()
    {
        Begin();

        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();
        //simple input controls
        if (VideoPlaybackState.IDLE == state)
        {
            if (0 < _videoFileName.Length)
            {
                bool loaded = _play.LoadVideo(_videoFileName); //local media in the app directory

                if (!loaded)
                {
                    _errorMessage = "Could not load [" + _videoFileName + "] (" + _play.GetLastError().ToString() + ")";
                    _success = false;
                    Result();
                    return;
                }
                loaded = _play.PlayLoaded(); //local media in the app directory
            }
            else
            {
                _errorMessage = "Missing filename";
                _success = false;
                Result();
            }
        }
    }
    override public void End()
    {
        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();
        if (VideoPlaybackState.END_OF_FILE == state)
        {
            ErrorCodes err = _play.GetLastError();
            if (ErrorCodes.OK != err)
            {
                _errorMessage = "Could not play: " + _play.GetLastError().ToString();
                _success = false;
            }
            else
            {
                _play.Stop();
                _success = true;
            }
            Result();
        }
        else
        {
            //test not yet ready //TODO: timeout?
        }
    }
}


/// <summary>
/// Test Video Playback Play, Pause, Elapsed time
/// </summary>
public class TestPlayPauseElapsedTime : Test
{
    private IOZOPlayer _play;
    private string _videoFileName;
    private int modeFrameCount = 0;
    private bool pauseTested = false;

    public TestPlayPauseElapsedTime(OZOPlayer player, string videoFileName)
    {
        _videoFileName = videoFileName;
        _play = player;
        _name = "TestPlayPauseElapsedTime";
    }
    override public void Start()
    {
        Begin();

        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();
        //simple input controls
        if (VideoPlaybackState.IDLE == state)
        {
            if (0 < _videoFileName.Length)
            {
                bool loaded = _play.LoadVideo(_videoFileName); //local media in the app directory

                if (!loaded)
                {
                    _errorMessage = "Could not load [" + _videoFileName + "] (" + _play.GetLastError().ToString() + ")";
                    _success = false;
                    Result();
                    return;
                }
                loaded = _play.PlayLoaded(); //local media in the app directory
            }
            else
            {
                _errorMessage = "Missing filename";
                _success = false;
                Result();
            }
        }
    }
    override public void End()
    {
        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();

        float timeSinceStart = _play.ElapsedTime() / 1000.0f;

        //pause
        if (VideoPlaybackState.PLAYING == state && !pauseTested && timeSinceStart>=2.0f)
        {
            modeFrameCount = 0;
            _play.Pause();
            pauseTested = true;
        }
        if (VideoPlaybackState.PAUSED == state && pauseTested && modeFrameCount++ >= 60*5)
        {
            _play.Resume();
        }

        //end test
        if (VideoPlaybackState.END_OF_FILE == state)
        {
            ErrorCodes err = _play.GetLastError();
            if (ErrorCodes.OK != err)
            {
                _errorMessage = "Could not play: " + _play.GetLastError().ToString();
                _success = false;
            }
            else
            {
                _success = pauseTested;
                _play.Stop();
            }
            Result();
        }
        else
        {
            //test not yet ready //TODO: timeout?
        }
    }
}

/// <summary>
/// Test Video Playback Seek
/// </summary>
public class TestPlaySeekForward : Test
{
    private IOZOPlayer _play;
    private string _videoFileName;
    private bool _seekTested = false;
    private ulong _seekStartTime = 0;

    public TestPlaySeekForward(OZOPlayer player, string videoFileName)
    {
        _videoFileName = videoFileName;
        _play = player;
        _name = "TestPlaySeekForward";
    }
    override public void Start()
    {
        Begin();

        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();
        //simple input controls
        if (VideoPlaybackState.IDLE == state)
        {
            if (0 < _videoFileName.Length)
            {
                bool loaded = _play.LoadVideo(_videoFileName); //local media in the app directory

                if (!loaded)
                {
                    _errorMessage = "Could not load [" + _videoFileName + "] (" + _play.GetLastError().ToString() + ")";
                    _success = false;
                    Result();
                    return;
                }
                loaded = _play.PlayLoaded(); //local media in the app directory
            }
            else
            {
                _errorMessage = "Missing filename";
                _success = false;
                Result();
            }
        }
    }
    override public void End()
    {
        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();

        ulong halfDuration = _play.Duration() / 2;
        ulong elapsed = _play.ElapsedTime();

        //pause
        if (VideoPlaybackState.PLAYING == state && !_seekTested && elapsed > 1000 && elapsed < 1500 )
        {
            _seekStartTime = elapsed;
            bool result = _play.SeekTo(halfDuration);
            _seekTested = result;
        }
        else if (VideoPlaybackState.PLAYING == state && _seekTested)
        {
            if(elapsed > _seekStartTime+100 && elapsed < halfDuration)
            {
                _errorMessage = "Did not seek at least to: " + (halfDuration) / 1000.0f + " (elapsed:" + elapsed + ")";
                _success = false;
                Result();
                return;
            }
        }

        //end test
        if (VideoPlaybackState.END_OF_FILE == state)
        {
            ErrorCodes err = _play.GetLastError();
            if (ErrorCodes.OK != err)
            {
                _errorMessage = "Could not play: " + _play.GetLastError().ToString();
                _success = false;
                Result();
            }
            else
            {
                _success = _seekTested;
                _play.Stop();
            }
            Result();
        }
        else
        {
            //test not yet ready //TODO: timeout?
        }
    }
}


/// <summary>
/// Test Video Playback Seek Backwards
/// </summary>
public class TestPlaySeekBackward : Test
{
    private IOZOPlayer _play;
    private string _videoFileName;
    private bool _seekTested = false;
    private ulong _seekStartTime = 0;

    public TestPlaySeekBackward(OZOPlayer player, string videoFileName)
    {
        _videoFileName = videoFileName;
        _play = player;
        _name = "TestPlaySeekBackward";
    }
    override public void Start()
    {
        Begin();

        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();
        //simple input controls
        if (VideoPlaybackState.IDLE == state)
        {
            if (0 < _videoFileName.Length)
            {
                bool loaded = _play.LoadVideo(_videoFileName); //local media in the app directory

                if (!loaded)
                {
                    _errorMessage = "Could not load [" + _videoFileName + "] (" + _play.GetLastError().ToString() + ")";
                    _success = false;
                    Result();
                    return;
                }
                loaded = _play.PlayLoaded(); //local media in the app directory
            }
            else
            {
                _errorMessage = "Missing filename";
                _success = false;
                Result();
            }
        }
    }
    override public void End()
    {
        VideoPlaybackState state = _play.GetCurrentVideoPlaybackState();

        ulong elapsed = _play.ElapsedTime();

        //pause
        if (VideoPlaybackState.PLAYING == state && !_seekTested && elapsed > 2000 && elapsed < 2500)
        {
            //Debug.Log("Elapsed: " + elapsed + "\n");
            _seekStartTime = elapsed;
            bool result = _play.SeekTo(elapsed - 1000);
            _seekTested = result;
        }
        else if (VideoPlaybackState.PLAYING == state && _seekTested)
        {
            if (elapsed < _seekStartTime-100 )
            {
                if(elapsed > 1900)
                {
                    _errorMessage = "Did not seek at most to: " + (1900) / 1000.0f + " (elapsed:" + elapsed + ")";
                    _success = false;
                }
                else
                {
                    ErrorCodes err = _play.GetLastError();
                    if (ErrorCodes.OK != err)
                    {
                        _errorMessage = "Seeking error: " + _play.GetLastError().ToString();
                        _success = false;
                    }
                    else
                    {
                        _success = _seekTested;
                        _play.Stop();
                    }                   
                }
                Result();
            }

        }       
        else
        {
            //test not yet ready //TODO: timeout?
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Test Runner
/// </summary>
public class BuildTestRun : MonoBehaviour
{
    private int _currentTest = 0;
    private System.Collections.Generic.List<Test> _testCases = new System.Collections.Generic.List<Test>();

    [SerializeField]
    private OZOPlayer _playerView = null;
    [SerializeField]
    private string[] m_VideoFiles = null;

    void Awake()
    {
        Test.BeginTests();
    }

    // Use this for initialization
    void Start()
    {
        if (0 == m_VideoFiles.Length)
        {
            Debug.Log("ERROR: No video files given\n");
            return;
        }

        //for editor convenience
        UnityEngine.VR.InputTracking.Recenter();

        //Add all the tests
        _testCases.Add(new TestInit(_playerView));
        _testCases.Add(new TestPlay(_playerView, m_VideoFiles[0]));
        _testCases.Add(new TestPlayPauseElapsedTime(_playerView, m_VideoFiles[0]));
        _testCases.Add(new TestPlaySeekForward(_playerView, m_VideoFiles[0]));
        _testCases.Add(new TestPlaySeekBackward(_playerView, m_VideoFiles[0]));
    }

    // Run through all the tests
    void Update ()
    {
        if(Test.isRunning)
        {
            Test test = _testCases[_currentTest];
            if (!test.isStarted)
            {
                test.Start();
            }
            else if (!test.isEnded)
            {
                test.End();
            }
            else
            {
                _currentTest++;
                if (_currentTest >= _testCases.Count)
                {
                    Test.EndTests();
                }
            }
        }
    }
}
