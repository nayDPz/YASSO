using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace YassoMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject bombPrefab;

        internal static void RegisterProjectiles()
        {
            // only separating into separate methods for my sanity
            CreateBomb();

            AddProjectile(bombPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("Fireball", "YassoTornadoProjectile");

            bombPrefab.transform.localScale = new Vector3(1, 1, 1);
            GameObject.Destroy(bombPrefab.GetComponent<SphereCollider>());
            GameObject.Destroy(bombPrefab.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileSimple s = bombPrefab.GetComponent<ProjectileSimple>();
            s.lifetime = 1.25f;
            s.desiredForwardSpeed = 50f;
            s.enableVelocityOverLifetime = true;
            s.velocityOverLifetime = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(0.85f, 0.1f), new Keyframe(1f, 0f) });

            ProjectileDamage d = bombPrefab.GetComponent<ProjectileDamage>();
            d.damageType = DamageType.ApplyMercExpose;

            GameObject hitbox = new GameObject("hitbox");
            hitbox.transform.parent = bombPrefab.transform;
            hitbox.transform.localScale = new Vector3(13f, 13f, 13f);
            hitbox.transform.localPosition = new Vector3(0, 0f, 0);

            HitBox[] hbs = new HitBox[] { hitbox.AddComponent<HitBox>() };
            bombPrefab.AddComponent<HitBoxGroup>().hitBoxes = hbs;
            ProjectileOverlapAttack o = bombPrefab.AddComponent<ProjectileOverlapAttack>();
            o.damageCoefficient = 1f;
            o.forceVector = Vector3.up * 3000f;


            o.impactEffect = Modules.Assets.tornadoImpactEffect;

            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("FUCKINGEGG") != null) bombController.ghostPrefab = CreateGhostPrefab("FUCKINGEGG");
            bombController.startSound = "YasuoTornadoStart";

            bombController.ghostPrefab.GetComponent<Renderer>().material = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercSwipe2.mat").WaitForCompletion();
            GameObject.Destroy(bombController.ghostPrefab.GetComponent<SphereCollider>());

            
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}