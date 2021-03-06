﻿using LeagueSandbox.GameServer.Logic.Packets;
using LeagueSandbox.GameServer.Logic.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic.Enet;
using LeagueSandbox.GameServer.PluginSystem.Faces;
using LeagueSandbox.GameServer.PluginSystem;

namespace LeagueSandbox.GameServer.Logic.GameObjects
{
    public enum EProjectileTargetType
    {
        TargetFallow,
        Normal,
        DefinationCallBack
    }

    public class Projectile : GameObject
    { 
        protected List<GameObject> objectsHit = new List<GameObject>();
        protected Spell originSpell;
        protected Unit owner;
        protected float missileSpeed;
        protected float missileMinSpeed;
        protected float missileMaxSpeed;
        protected int projectileId;
        protected int flags;
        protected EProjectileTargetType targetType;
        protected float _startX;
        protected float _startY;

        public Projectile(Game game, uint id, float x, float y, int collisionRadius, Unit owner, Target target, Spell originSpell, float moveSpeed, int projectileId, EProjectileTargetType targetType, int flags = 0) : base(game, id, x, y, collisionRadius)
        { 
            this.originSpell = originSpell;
            this.missileSpeed = moveSpeed;
            this.owner = owner;
            this.projectileId = projectileId;
            this.flags = flags; 
            setTarget(target);
            this._startX = x;
            this._startY = y;
             
            if (!target.isSimpleTarget())
                ((GameObject)target).incrementAttackerCount();

            owner.incrementAttackerCount();
        }

        #region Overrides
        public override void update(long diff)
        {
            if (target == null)
            {
                setToRemove();
                return;
            }

            if (target.isSimpleTarget())
            { // Skillshot
                var objects = _game.GetMap().GetObjects();
                foreach (var it in objects)
                {
                    if (isToRemove())
                        return;

                    if (collide(it.Value))
                    {
                        if (objectsHit.Contains(it.Value))
                            continue;

                        var u = it.Value as Unit;
                        if (u == null)
                            continue;

                        if (u.getTeam() == owner.getTeam() && !((flags & (int)SpellFlag.SPELL_FLAG_AffectFriends) > 0))
                            continue;

                        if (u.getTeam() == TeamId.TEAM_NEUTRAL && !((flags & (int)SpellFlag.SPELL_FLAG_AffectNeutral) > 0))
                            continue;

                        if (u.getTeam() != owner.getTeam() && u.getTeam() != TeamId.TEAM_NEUTRAL && !((flags & (int)SpellFlag.SPELL_FLAG_AffectEnemies) > 0))
                            continue;


                        if (u.isDead() && !((flags & (int)SpellFlag.SPELL_FLAG_AffectDead) > 0))
                            continue;

                        var m = u as Minion;
                        if (m != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectMinions) > 0))
                            continue;

                        var p = u as Placeable;
                        if (p != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectUseable) > 0))
                            continue;

                        var t = u as Turret;
                        if (t != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectTurrets) > 0))
                            continue;

                        var i = u as Inhibitor;
                        if (i != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectBuildings) > 0))
                            continue;

                        var n = u as Nexus;
                        if (n != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectBuildings) > 0))
                            continue;

                        var c = u as Champion;
                        if (c != null && !((flags & (int)SpellFlag.SPELL_FLAG_AffectHeroes) > 0))
                            continue;

                        objectsHit.Add(u);
                        originSpell.applyEffects(u, this);
                    }
                }
            }
            else
            {
                var u = target as Unit;
                if (u != null && collide(u))
                { // Autoguided spell
                    if (originSpell != null)
                    {
                        originSpell.applyEffects(u, this);
                    }
                    else
                    { // auto attack
                        owner.autoAttackHit(u);
                        setToRemove();
                    }
                }
            }

            if (this.targetType == EProjectileTargetType.DefinationCallBack && this.target != null && defination(this.target))
            {
                if (this.getOwner().getPlugin().Loaded)
                {
                    try
                    { 
                        short spellSlot = this.originSpell == null ? (short)-1 : this.originSpell.getSlot();
                        this.getOwner().getPlugin().getContent<IUnit>().onDefinationProjectTile(this.originSpell, spellSlot, this, this.target.getX(), this.target.getY());
                    }
                    catch (Exception ex)
                    {
                        PluginManager.Exception(ex, this.getOwner().getPlugin());
                    } 
                } 
            }

            base.update(diff);
        } 

        public override float getMoveSpeed()
        {
            return missileSpeed;
        }

        public override void setToRemove()
        {
            if (target != null && !target.isSimpleTarget())
                (target as GameObject).decrementAttackerCount();

            owner.decrementAttackerCount();
            base.setToRemove();
            _game.PacketNotifier.notifyProjectileDestroy(this);
        }
        #endregion

        #region Gets
        public Unit getOwner()
        {
            return owner;
        }

        public List<GameObject> getObjectsHit()
        {
            return objectsHit;
        }
         
        public int getProjectileId()
        {
            return projectileId;
        }

        public Spell getSpell()
        {
            return this.originSpell;
        }

        public float getMissileSpeed()
        {
            return this.missileSpeed;
        }

        public float getMissileMinSpeed()
        {
            return this.missileMinSpeed;
        }

        public float getMissileMaxSpeed()
        {
            return this.missileMaxSpeed;
        }

        public EProjectileTargetType getTargetType()
        {
            return targetType;
        }

        public Unit getTargetUnit()
        {
            return this.target as Unit;
        }

        public float getStartX()
        {
            return _startX;
        }

        public float getStartY()
        {
            return _startX;
        }
        #endregion

        #region Sets
        public Projectile setMissileMaxSpeed(float value)
        {
            this.missileMaxSpeed = value;
            return this;
        }

        public Projectile setMissileMinSpeed(float value)
        {
            this.missileMinSpeed = value;
            return this;
        }

        public Projectile setMissileSpeed(float value)
        {
            this.missileSpeed = value;
            return this;
        }

        public Projectile setStartX(float value)
        {
            this._startX = value;
            return this;
        }

        public Projectile setStartY(float value)
        {
            this._startY = value;
            return this;
        }

        public Projectile setStartPosition(float X, float Y)
        {
            this._startX = X;
            this._startY = Y;
            return this;
        }
        #endregion

        #region
        public void Notify()
        {
            GetGame().GetMap().AddObject(this);
            GetGame().PacketNotifier.notifyProjectileSpawn(this);
        }
        #endregion
    }
}
