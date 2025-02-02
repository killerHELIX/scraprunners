#pragma once

#include "CoreMinimal.h"
#include "BiomeData.h"

class SCRAPRUNNERS_API Biome
{
public:
	Biome();
	virtual ~Biome() = default;
	virtual FBiomeData GetBiomeData() = 0;
	virtual void GenerateTerrainData(TArray<FVector>& OutVertexPositions, TArray<FVector>& OutNormals, TArray<FVector2D>& OutUVs, TArray<int32>& OutIndices) {}
};
