// Fill out your copyright notice in the Description page of Project Settings.


#include "DesertBiome.h"

DesertBiome::DesertBiome()
{
}

DesertBiome::~DesertBiome()
{
}

FBiomeData DesertBiome::GetBiomeData()
{
	FBiomeData BiomeData;
	//todo put in numbers;
	return BiomeData;
}

void DesertBiome::GenerateTerrainData(TArray<FVector>& OutVertexPositions,
	TArray<FVector>& OutNormals,
	TArray<FVector2D>& OutUVs,
	TArray<int32>& OutIndices)
{
	//todo procedural content algorithm
}
