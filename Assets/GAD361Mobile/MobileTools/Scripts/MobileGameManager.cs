using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Murdoch.GAD361.MobileTools
{
    /**
    * \class MobileGameManager
    * A base class for a mobile game manager, which does the following things:
    * - Acts as a singleton, meaning any other instances of this class in an 
    *    active scene will destroy themselves, leaving only the original.
    *    It's not a true singleton (doesn't have private constructor, can be subclassed,
    *    is not thread safe etc) but this is OK since we know this for use on
    *    Unity gameobjects (we leverage Awake() ) and in a single thread.
    * - Has a static _instance variable which can be accessed via the static 
    *    instance property.
    * - isDesktop is a flag to tell input detectors to use 'desktop' methods
    *    of collecting input (such as keyboard) for testing purposes.
    * - You can get the current scene name with CurrentScene
    * - You can override OnPauseChanged() to take action when the pause state changes
    * - You can override OnSceneChanged() to take action when the scene changes.
    * -  EnableTouchControls() turns touch controls on or off
    * - You can call SceneTransition() with various parameters to switch scenes.
    */
    public class MobileGameManager : MonoBehaviour
    {
        protected static MobileGameManager _instance;
        public RenderPipelineAsset renderPipelineAsset;
        public bool isDesktop; //this will show in inspector.
        protected bool isPaused; //is the game paused?
        public delegate void OnSceneLoaded();

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            
        }

        /**
        * The static instance property, returns the global singleton instance.
        * This will be of type MobileGameManager, derived classes will need
        * to cast this.
        * i.e. ((MyManager)(MyManager.instance)).someMyManagerFunction();
        * A good option may be for a derived class to implement its own GetInstance:
        * 
        *     public static MyDerivedClass GetInstance()
        *     {
        *         return (MyDerivedClass)this.instance;   
        *     }
        *
        */
        public static MobileGameManager instance
        {
            get {return _instance;}
        }

        /**
        * This will allow anyone to get 'is Desktop'
        */
        public static bool IsDesktop
        {
            get { return _instance.isDesktop; }
        }

        /**
        * Tells you if you are Paused
        */
        public static bool Paused
        {
            get { return _instance.isPaused; }
            set { _instance.isPaused = value; }
        }

        /**
        * Gives you the current scene name.
        */
        public string currentSceneName
        {
            get { return SceneManager.GetActiveScene().name; }
        } 

        /**
        * Called by the first instanced singleton.
        * Overriding implementations should consider calling
        * base.Init(), as this sets the correct render pipeline and
        * registers OnSceneChanged() for scene changes with Unity.
        */
        protected virtual void Init()
        {
            if (renderPipelineAsset)
            {
                GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
            }
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            Debug.Log("Running on mobile platform?" + Application.isMobilePlatform);
            if (Application.isMobilePlatform)
            {
                if (_instance.isDesktop)
                {
                    Debug.Log("Disabling 'isDesktop' test mode");
                    _instance.isDesktop = false;
                }
            }
            //Dont need to call on scene changed, this seems to happen automatically.
            //OnSceneChanged();
        }

        /**
        * Call this to trigger a change in 'onPaused' status
        * @see OnPauseChanged()
        */
        public virtual void Pause(bool pause)
        {
            isPaused = pause;
            OnPauseChanged();
        }

        /**
        * Override this to do something when 'isPaused' changes
        */
        public virtual void OnPauseChanged()
        {
            //override this
        }

        public virtual void OnActiveSceneChanged(Scene prev, Scene current)
        {
            OnSceneChanged();
        }

        /**
        * Override this to do something when a scene changes.
        */
        public virtual void OnSceneChanged()
        {

        }

        public virtual void ResizeOrthoCamera(Camera cam = null)
        {
            //Ortho size is half size of vertical viewing volume.
            //If we know a reference res, calculate that aspect ratio,
            //and set ortho size to prioritise the important dimention,
            //either width or height
            if (cam == null)
                cam = Camera.main;
            Debug.Log(cam.orthographicSize);
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
            //Assets made for 1140/11.62 size = 98.1
            //1440/2560
            Vector2 refRes = new Vector2(768, 1080);
            float ratio = refRes.x/Screen.width; //(Screen.width*1.0f)/(Screen.height*1.0f);
            Debug.Log("Desired width = " + refRes.x);
            Debug.Log("Ratio: " + ratio);
            //Debug.Log("Ratio * desired width = " + refRes.x * ratio);
            Debug.Log("height needed for width of " + refRes.x + " = " + refRes.y * ratio);
            //cam.orthographicSize = 1140.0f/98.1f;
            Debug.Log("Setting ortho size of " + cam.orthographicSize);
        }

        public virtual void EnableTouchControls(bool shouldEnable=true)
        {
            MobileControls.controlsEnabled = shouldEnable;
        }

        /**
        * Call this function to change scenes, giving the new scene name.
        * newSceneName must be one of the scenes in the build settings.
        * Optionally you can pass transition, which is an animator you have
        * set up somewhere in the sub structure of your GameManager. If this is
        * passed, SceneTransition will set the trigger (triggerName)
        * on the passed animator, presumably calling some transition animation you
        * have made. In the case that there is a need to wait a little while before
        * loading in the new scene (e.g. while the animation is playing), 
        * you can set waitTime to a positive value.
        * @param newSceneName must be one of the scenes in the build settings.
        * @param transition an optional Animator
        * @param triggerName the trigger to fire on the transition Animator
        * @param waitTime amount of seconds to wait before loading the new scene.
        */
        public virtual void SceneTransition(string newSceneName, Animator transition = null, string triggerName = "ChangeScene", float waitTime = 0.0f)
        {
            //Without transition
            if (transition == null)
            {
                SceneManager.LoadScene(newSceneName);
            }
            else //with transition
            {
                StartCoroutine( TransitionToScene(newSceneName, transition, triggerName, waitTime ) );
            }
        }

        IEnumerator TransitionToScene(string newSceneName, Animator transition, string triggerName, float waitTime) //, OnSceneLoaded sceneLoadedFunction)
        {
            transition.SetTrigger(triggerName);
            //Debug.Log("Transitioning");
            yield return new WaitForSecondsRealtime(waitTime);
            SceneManager.LoadScene(newSceneName);
            //Don't need to do this because we subscribe.
            //OnSceneChanged()
        }
    }
}
