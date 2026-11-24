using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif


public class coroutineAnimator : MonoBehaviour
{
    public bool IsFull => queue.Full;
    public bool IsEmpty => queue.Empty;

    public void SetQueueLength(int i)
    {
        queue = new Buffer<Animation>(i);
    }

    Buffer<Animation> queue = new Buffer<Animation>(8);
    public Coroutine last;
    public Animation Current;
    public bool IsAnimating => Coroutines.InstanceExists && last != null && !last.Finished;
    public bool PlayAnim(IEnumerator<float> anim)
    {

        Animation a = new Animation();
        a.anim = anim;

        if (!IsAnimating)
        {
           
            Current = a;
            last = Coroutines.RunCoroutine(anim.CancelWith(gameObject));
            last.End += End;
            return true;
        } else
        {
            if (!queue.Full)
            {
                queue.addBuffer(a);
                return true;
            }
            return false;
        }
    }
    public bool PlayAnimGetAnim(IEnumerator<float> anim, out Animation a)
    {
        a = new Animation();
        a.anim = anim;

        if (!IsAnimating)
        {
            Current = a;
            last = Coroutines.RunCoroutine(anim.CancelWith(gameObject));
            last.End += End;
            return true;
        }
        else
        {

            if (!queue.Full)
            {
                queue.addBuffer(a);
                return true;
            }
            return false;
        }
    }
    public bool PlayAnim(Animation anim)
    {

        if (!IsAnimating)
        {
           
            return true;
        }
        else
        {
            if (!queue.Full)
            {
                queue.addBuffer(anim);
                return true;
            }
            return false;
        }
    }
    void End()
    {
        if (Current != null) Current.OnFinish?.Invoke();

        if (!queue.Empty)
        {
            if (this != null)
            {
                Current = queue.nextBuffer();
                last = Coroutines.RunCoroutine(Current.anim.CancelWith(gameObject));
                last.End += End;
            }
            
        } else
        {
            Current = null;
        }
    }

    public void Cancel()
    {
        last?.Cancel();
    }

    public void ResetFinishingAnim()
    {
        SetQueueLength(8);
    }
}

//public class scaledCoroutineAnimator : coroutineAnimator
//{
//    public override IEnumerator<float> CancelWith(IEnumerator<float> handle, GameObject g)
//    {
//        return MEC.MECExtensionMethods2.CancelWith(handle, g);
//    }

//    public override void KillCoroutines(object handle)
//    {
//        Timing.KillCoroutines((MEC.CoroutineHandle)handle);
//    }

//    public override object RunCoroutine(IEnumerator<float> handle)
//    {
//        return Timing.RunCoroutine(handle);
//    }
//}
//public class realTimeCoroutineAnimator : coroutineAnimator
//{
//    public override IEnumerator<float> CancelWith(IEnumerator<float> handle, GameObject g)
//    {
//        return handle;
//        return RealMEC.MECExtensionMethods2.CancelWith(handle, g);
//    }

//    public override void KillCoroutines(object handle)
//    {
//        RealTiming.KillCoroutines((RealMEC.CoroutineHandle)handle);
//    }

//    public override object RunCoroutine(IEnumerator<float> handle)
//    {
//        return Coroutines.RunCoroutine(handle);
//    }
//}
[System.Serializable]
public class Animation
{
    public Action OnFinish;
    public Action OnStart;
    public IEnumerator<float>  anim;

}
#if UNITY_EDITOR
namespace UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(coroutineAnimator), true)]
    public class coroutineAnimatorCustomEditorClass : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            coroutineAnimator source = target as coroutineAnimator;
            GUILayout.Label(source.IsAnimating ? "ANIMATING" : "NOT ANIMATING");
            base.OnInspectorGUI();
        }
    }
}
#endif