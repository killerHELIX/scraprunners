// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "FastNoiseLite.h"

/**
 * 
 */
class SCRAPRUNNERS_API NoiseMapGenerator
{
public:
	NoiseMapGenerator();
	~NoiseMapGenerator();

	int SeedHeight = 0;
	int SeedMoisture = 0;
	FastNoiseLite Noise;
	TMap<FVector2D, float> HeightMap;
	TMap<FVector2D, float> MoistureMap;

	TMap<FVector2D, float> GenerateHeightMap(int Dimension);
	TMap<FVector2D, float> GenerateMoistureMap(int Dimension);
};
