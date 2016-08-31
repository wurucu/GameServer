using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.PluginSystem.Faces
{
    public interface IUnit
    { 
        void Initialize(Game game, Unit champion);
        void onStartCasting(Spell spell, float X, float Y, Target target, uint futureProjNetID, uint spellNetID);
        void onFinishCasting(Spell spell, float X, float Y, Target target, uint futureProjNetID, uint spellNetID);
        void onDefinationProjectTile(Spell spell, short SpellSlot, Projectile projectTile, float X, float Y);
        void onHit(Spell spell, short SpellSlot, Projectile projectTile, Unit unit);
        void onDie(Unit Killer);
        void onRecieveDamage(float Damage, DamageType damageType, DamageSource damageSource, bool isCritic);
        float onDeliverDamage(Unit target, float Damage, DamageText damageText, DamageType damageType, DamageSource damageSource);
        void onKilled(Unit unit);
        void onCollide(GameObject gameObject);
        void onLevelUpSpell(short slot);
        void onUpdate(long diff);
        float onAutoAttack(Unit target, float damage, bool nextAutoIsCrit);
        void onSpawn();
    }
}
