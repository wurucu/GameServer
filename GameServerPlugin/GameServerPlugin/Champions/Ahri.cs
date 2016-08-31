using LeagueSandbox.GameServer.PluginSystem.Faces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;
using System.Numerics;

namespace GameServerPlugin
{
    public class Ahri : IUnit
    {
        Game _game;
        Champion _champion;
        public void Initialize(Game game, Unit champion)
        {
            _champion = champion as Champion;
            _game = game; 
        }

        public float onAutoAttack(Unit target, float damage, bool nextAutoIsCrit)
        {
            return damage;  
        }

        public void onCollide(GameObject gameObject)
        {
              
        }

        public void onDefinationProjectTile(Spell spell, short SpellSlot, Projectile projectTile, float X, float Y)
        {
            //Hedefe ulaştı
            _champion.SendMessage("onDefinationProjectile");
            projectTile.setToRemove();
            var p = spell.newProjectileTarget("AhriOrbReturn", _champion, EProjectileTargetType.TargetFallow);
            p.setMissileSpeed(60).setMissileMinSpeed(60).setMissileMaxSpeed(2600);
            p.setStartX(X).setStartY(Y);
            p.Notify();
        }

        public float onDeliverDamage(Unit target, float Damage, DamageText damageText, DamageType damageType, DamageSource damageSource)
        {
            return Damage;
        }

        public void onDie(Unit Killer)
        {
            _champion.SendMessage("onDie");
        }

        public void onFinishCasting(Spell spell, float X, float Y, Target target, uint futureProjNetID, uint spellNetID)
        {
            if (spell != null)
            {
                if (spell.getSlot() == 0)
                { // Q

                    Vector2 current = new Vector2(_champion.getX(), _champion.getY());
                    Vector2 to = Vector2.Normalize(new Vector2(X, Y) - current);
                    Vector2 range = to * 1150;
                    Vector2 trueCoords = current + range;
                    var p = spell.newProjectile("AhriOrbMissile", trueCoords.X, trueCoords.Y, EProjectileTargetType.DefinationCallBack);
                    p.setMissileSpeed(2500).setMissileMinSpeed(400).setMissileMaxSpeed(2500);
                    p.Notify();
                }
                else if (spell.getSlot() == 1)
                { // W
                    _champion.SendMessage("W");
                }
                else if (spell.getSlot() == 2)
                { // E
                    _champion.SendMessage("E");
                }
                else if (spell.getSlot() == 3)
                { // R
                    _champion.SendMessage("R");
                }
                else if (spell.getSlot() == 4)
                { // D - Heal
                    _champion.SendMessage("Heal");
                }
                else if (spell.getSlot() == 5)
                { // F - Flash
                    _champion.SendMessage("Flash");
                } 
                else if (spell.getSlot() == 13)
                { // B - Recall
                    _champion.SendMessage("Recall");
                }
            }
            else
                _champion.SendMessage("Spell null");

        }

        public void onHit(Spell spell, short SpellSlot, Projectile projectTile, Unit unit)
        {
             
        }

        public void onKilled(Unit unit)
        {
             
        } 

        public void onLevelUpSpell(short slot)
        {
            
        }

        public void onRecieveDamage(float Damage, DamageType damageType, DamageSource damageSource, bool isCritic)
        {
            
        }

        public void onStartCasting(Spell spell, float X, float Y, Target target, uint futureProjNetID, uint spellNetID)
        {
            _champion.SendMessage("onStartCasting");
        }

        public void onUpdate(long diff)
        {
            
        }

        public void onSpawn()
        {
            _champion.SendMessage("onSpawn");
        }
    }
}
