using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickFlag : SingletonMonoBehaviour<GimmickFlag>
{
    // 仕掛けを解けるかどうか判別するための変数
    [System.NonSerialized] public bool canSolveCrateArrow;
    [System.NonSerialized] public bool canSolveTelescope;

    void Start()
    {
        canSolveCrateArrow = SaveManager.Instance.GetCanSolveGimmickFlag(Gimmick.Type.CrateArrow);
        canSolveTelescope = SaveManager.Instance.GetCanSolveGimmickFlag(Gimmick.Type.Telescope);
    }
}
