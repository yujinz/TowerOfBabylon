using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterClockwiseSeq : ClockwiseSeq {

    override public void setSeq(int num) {
        for (int i=0; i< len; i++) {
            seq[i] = num;
            num++;
            if (num == 9) num = 1;
        }
    }

    override public void setHammerDir() {
        Hero.S.HammerLeft = false;
    }

    override public void callHit() {
        Hero.S.hitCounterClockwise();
    }

    override public void callJump() {
        Hero.S.startJumpCounterClockwise();
    }
}
