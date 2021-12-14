using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{

	World world;
	Text text;

	float frameRate;
	float timer;

	int halfWorldSizeInVoxels;
	int halfWorldSizeInChunks;

	void Start ()
	{
		world = GameObject.Find("World").GetComponent<World>();
		text = GetComponent<Text>();

		halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
		halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
	}

	void Update ()
	{
		string debugText = "MineT (c) @itmaxxx 2020-2021";
		debugText += "\n";
		debugText += frameRate + " FPS";
		debugText += "\n";
		debugText += "Player X " + Mathf.FloorToInt(world.player.transform.position.x - halfWorldSizeInVoxels) + " Y " + Mathf.FloorToInt(world.player.transform.position.y) + " Z " + Mathf.FloorToInt(world.player.transform.position.z - halfWorldSizeInVoxels);
		debugText += "\n";
		debugText += "Chunk X " + Mathf.FloorToInt(world.playerChunkCoord.x - halfWorldSizeInChunks) + " Z " + Mathf.FloorToInt(world.playerChunkCoord.z - halfWorldSizeInChunks);

		text.text = debugText;

		if (timer > 1f)
		{
			frameRate = (int)(1f / Time.unscaledDeltaTime);
			timer = 0;
		}
		else
		{
			timer += Time.deltaTime;
		}
	}

}
