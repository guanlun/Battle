﻿using UnityEngine;
using System.Collections.Generic;

public class KnightBehavior : SoldierBehavior {
    private int frameCount = 0;
    private int fallDownFrameCount = 40;

    protected override void init() {
        this.hp = 300;
    }

    void Start () {
        init();

        navMeshAgent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        anim.speed = 0.5f;
        // anim.enabled = false;

        Transform[] parts = GetComponentsInChildren<Transform>();
        foreach (Transform part in parts) {
            if (part.name == "Weapon") // Weapon name
            {
                this.weaponParent = part;
                attackRange = 0f;

                this.weapon = (GameObject)Instantiate(Resources.Load("LancePrefab"));
                this.weapon.transform.parent = part.transform;
                this.weapon.transform.localPosition = new Vector3(0, 0, 0);
                this.weapon.transform.localScale = new Vector3(1, 1, 1);
                this.weapon.transform.localRotation = Quaternion.identity;

                this.weaponBehavior = this.weapon.GetComponent<WeaponBehavior>();
                if (this.weaponBehavior != null) {
                    this.weaponBehavior.team = this.team;
                    this.weaponBehavior.holder = this;
                }
            } else if (part.name == "Object02") {
                Renderer renderer = part.GetComponent<Renderer>();

                string matType = team == "red" ? "RedArmyMat" : "BlueArmyMat";
                renderer.material = Resources.Load(matType, typeof(Material)) as Material;
            }
        }
    }
	
	void Update () {
        if (StateManager.paused) {
            return;
        }

        if (!alive) {
            if (fallDownFrameCount == 0) {
                transform.rotation = Quaternion.Slerp(transform.rotation, fallenTargetRotation, 0.1f);
                return;
            } else {
                fallDownFrameCount--;
            }
        }

        FindTarget();

        if (target == null) {
            if (this.frameCount == 0) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.right), 0.01f);
            } else {
                this.navMeshAgent.destination = this.transform.position + this.transform.forward * 20;

                this.frameCount--;
            }
        } else {
            navMeshAgent.destination = target.position;
            this.frameCount = 30;
        }
    }

    protected override void gettingKilled() {
        transform.Rotate(new Vector3(0, 0, 90));
        fallenTargetRotation = transform.rotation;

        // Transform back
        transform.Rotate(new Vector3(0, 0, -90));
    }

    protected override void FindTarget() {
        float closestDist = float.MaxValue;
        SoldierBehavior closestAgent = null;

        foreach (SoldierBehavior behavior in StateManager.soldierBehaviors) {
            if (!behavior.alive || behavior.team == team) {
                continue;
            }

            Vector3 enemyPosition = behavior.transform.position;

            Vector3 targetDirection = Vector3.Normalize(enemyPosition - this.transform.position);
            Vector3 movingDirection = Vector3.Normalize(this.transform.forward);

            if (Vector3.Dot(targetDirection, movingDirection) < 0.5) {
                continue;
            }

            float dist = Vector3.Distance(enemyPosition, transform.position);

            if (dist < closestDist) {
                closestDist = dist;
                closestAgent = behavior;
            }
        }

        if (closestAgent != null) {
            target = closestAgent.transform;
        } else {
            target = null;
        }
    }
}
