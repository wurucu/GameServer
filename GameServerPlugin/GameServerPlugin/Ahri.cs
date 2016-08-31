using LeagueSandbox.GameServer.PluginSystem.Faces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;

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
            
        }

        public float onDeliverDamage(Unit target, float Damage, DamageText damageText, DamageType damageType, DamageSource damageSource)
        {
            return Damage;
        }

        public void onDie(Unit Killer)
        {
            
        }

        public void onFinishCasting(Spell spell, float X, float Y, Target target, uint futureProjNetID, uint spellNetID)
        {
            _champion.SendMessage("onFinishCasting");
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
    }
}
