using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������غ��л��߼�, ͳ�ƴ�����ܸ���ģ��Ľ��
public class RoundManager : MonoBehaviour
{
    public int CurrentRound { get; private set; }

    private StorylineManager storylineManager;
    private ExploreManager exploreManager;

    public void Start()
    {
        CurrentRound = 1;
        storylineManager = GetComponent<StorylineManager>();
        exploreManager = GetComponent<ExploreManager>();
    }

    //��һ�غϳ�ʼ��
    public void InitRound()
    {

    }

    //��Ҫ�������ѡ���Ƿ�ѡ�����
    public bool NextRound()
    {
        CurrentRound += 1;
        return true;
    }
}
