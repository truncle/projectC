using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用来管理回合切换逻辑, 统计处理汇总各个模块的结果
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

    //第一回合初始化
    public void InitRound()
    {

    }

    //需要检查所有选项是否选择完毕
    public bool NextRound()
    {
        CurrentRound += 1;
        return true;
    }
}
