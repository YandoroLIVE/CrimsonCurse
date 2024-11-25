using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Spriter2UnityDX.Editor
{
    public partial class PrefabBuilder
    {
        private Dictionary<string, Transform> transforms = new Dictionary<string, Transform>();
        private Dictionary<string, SpatialInfo> defaultBones = new Dictionary<string, SpatialInfo>();
        private Dictionary<string, SpriteInfo> defaultSprites = new Dictionary<string, SpriteInfo>();

        public void BuildPrefab(ScmlData data, Transform parent)
        {
            var folders = data.Folders;
            var entity = data.Entities[0];
            var entityObject = new GameObject(entity.name).transform;
            entityObject.SetParent(parent);

            var parents = new Dictionary<int, string>();
            parents[-1] = entityObject.name;

            foreach (var bone in entity.bones)
            {
                var name = bone.name;
                var parentName = parents[bone.parent];
                var parentTransform = transforms[parentName];

                var boneObject = new GameObject(name).transform;
                boneObject.SetParent(parentTransform);

                transforms[name] = boneObject;
                defaultBones[name] = bone;
            }
            foreach (var anim in entity.animations)
            {
                var animationName = anim.name;
                var timelineObjects = anim.timelines;
                var timelineTransforms = new Dictionary<int, Transform>();

                foreach (var timeline in timelineObjects)
                {
                    var id = timeline.id;
                    var name = timeline.name;

                    var timelineObject = new GameObject(name).transform;
                    timelineObject.SetParent(entityObject);

                    timelineTransforms[id] = timelineObject;
                    transforms[name] = timelineObject;

                    var spriteInfo = defaultSprites[name] = timeline.keys[0].info;
                    var spriteRenderer = timelineObject.gameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = folders[spriteInfo.folder][spriteInfo.file];
                }
            }
        }
    }
}

