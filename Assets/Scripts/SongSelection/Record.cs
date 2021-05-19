//using UnityEngine;
//using UnityEditor.Animations;

//public class Record : MonoBehaviour
//{
//    public AnimationClip clip;

//    private GameObjectRecorder m_Recorder;

//    void Start()
//    {

//        m_Recorder = new GameObjectRecorder(gameObject);

//        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
//    }

//    void LateUpdate()
//    {
//        if (clip == null)
//            return;


//        m_Recorder.TakeSnapshot(Time.deltaTime);
//    }

//    void OnDisable()
//    {
//        if (clip == null)
//            return;

//        if (m_Recorder.isRecording)
//        {
//            m_Recorder.SaveToClip(clip);
//        }
//    }
//}

