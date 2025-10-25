using UnityEngine;
using DG.Tweening;
public class TweenStairs : MonoBehaviour
{
    public void StartTweenStairs()
    {
        transform.DOLocalMoveY(0, 5);
    }
}
