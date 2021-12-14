using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    
    public static void MakeTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight)
    {
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.y), 250f, 3f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        int maxLeavesWidth = 3;

        for (int x = -maxLeavesWidth; x <= maxLeavesWidth; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int z = -maxLeavesWidth; z <= maxLeavesWidth; z++)
                {
                    if ((x != -maxLeavesWidth && z != -maxLeavesWidth) ||
                        (x != -maxLeavesWidth && z != maxLeavesWidth) ||
                        (x != maxLeavesWidth && z != -maxLeavesWidth) ||
                        (x != maxLeavesWidth && z != maxLeavesWidth))
                        queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - (int)(height * 0.35f) + y, position.z + z), 8));
                }
            }
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 9));
        }
    }

}
