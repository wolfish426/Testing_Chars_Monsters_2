using UnityEngine;
using System.Collections.Generic;
using Spine;
using Spine.Unity.AttachmentTools;
using Spine.Unity;

public class BaseAtlasRegionAttacher : MonoBehaviour
{
	protected string[] _skins;
	[System.Serializable]
	public class SlotRegionPair {
		[SpineSlot] public string slot;
		[SpineAtlasRegion] public string region;
	}

	[SerializeField] protected SpineAtlasAsset atlasAsset;
	[SerializeField] protected bool inheritProperties = true;
	[SerializeField] protected List<SlotRegionPair> attachments = new List<SlotRegionPair>();

	protected Texture2D tex = null;
	protected float buttonSize = 160f;

	public virtual void Awake() {
		var skeletonRenderer = GetComponent<SkeletonRenderer>();
		skeletonRenderer.OnRebuild += Apply;
		if (skeletonRenderer.valid) Apply(skeletonRenderer);
	}

	void Start() {
		tex = new Texture2D(1, 1);
		tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
		tex.Apply();
	}

	void Apply(SkeletonRenderer skeletonRenderer) {
		if (!enabled) return;

		var atlas = atlasAsset.GetAtlas();
		if (atlas == null) return;
		var scale = skeletonRenderer.skeletonDataAsset.scale;

		foreach (var entry in attachments)
		{
			var skin = skeletonRenderer.Skeleton.Skin;
			Slot slot = skeletonRenderer.Skeleton.FindSlot(entry.slot);
			Attachment originalAttachment = slot.Attachment;
			AtlasRegion region = atlas.FindRegion(entry.region);
			var slotIndex = skeletonRenderer.skeleton.FindSlotIndex(entry.slot);

			if (region == null)
			{
				slot.Attachment = null;
				skin.RemoveAttachment(slotIndex, entry.slot);
			}
			else if (inheritProperties && originalAttachment != null)
			{
				var attach = originalAttachment.GetRemappedClone(region, false, true, scale);
				skin.SetAttachment(slotIndex, entry.slot, attach);
				slot.Attachment = attach;
			}
		}
	}

	public void OnSkinChanged() {
		var skeletonRenderer = GetComponent<SkeletonRenderer>();
		if (skeletonRenderer.valid) Apply(skeletonRenderer);
		skeletonRenderer.Skeleton.SetSlotsToSetupPose();

	}
}
