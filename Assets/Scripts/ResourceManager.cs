using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用来管理每轮的资源变更
public class ResourceManager : MonoBehaviour
{
    public Object CurrentResource;
    public Object TempResource;

    //增减资源, 只修改TempResource
    public bool ChangeResource(Object ResourceChange)
    {
        //检查资源是否足够
        //修改TempResource
        return true;
    }

    //将资源变更同步到当前资源
    public void SyncResource()
    {
    }
}
