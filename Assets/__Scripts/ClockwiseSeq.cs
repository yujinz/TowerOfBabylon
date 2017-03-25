using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockwiseSeq : RotateSeq {
    override public void setSeq(int num) {
        for (int i = 0; i < len; i++) {
            seq[i] = num;
            num--;
            if (num == 0) num = 8;
        }
    }

    override public void setHammerDir() {
        Hero.S.HammerLeft = true;
    }

    override public void callHit() {
        Hero.S.hitClockwise();
    }

    override public void callJump() {
        Hero.S.startJumpClockwise();
    }

}
