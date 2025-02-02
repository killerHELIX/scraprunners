// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Biome.h"

/**
 * 
 */
class SCRAPRUNNERS_API DesertBiome : public Biome
{
public:
	DesertBiome();
	virtual ~DesertBiome() = default;

	virtual FBiomeData GetBiomeData() override;

	virtual void GenerateTerrainData(TArray<FVector>& OutVertexPositions, TArray<FVector>& OutNormals, TArray<FVector2D>& OutUVs, TArray<int32>& OutIndices) override;
};
