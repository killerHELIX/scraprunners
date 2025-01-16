// Fill out your copyright notice in the Description page of Project Settings.


#include "NoiseMapGenerator.h"
#include "Kismet/KismetMathLibrary.h"

NoiseMapGenerator::NoiseMapGenerator()
{
	
}

TMap<FVector2D, float> NoiseMapGenerator::GenerateHeightMap(int Dimension)
{
	SeedHeight = FMath::Rand();
	Noise.SetNoiseType(FastNoiseLite::NoiseType_Perlin);
	Noise.SetSeed(SeedHeight);

	for (int X = 0; X < Dimension; X++)
	{
		for (int Y = 0; Y < Dimension; Y++)
		{
			HeightMap.Add(FVector2D((float)X, (float)Y), Noise.GetNoise((float)X, (float)Y));
		}
	}
	return HeightMap;
}

TMap<FVector2D, float> NoiseMapGenerator::GenerateMoistureMap(int Dimension)
{
	SeedMoisture = FMath::Rand();
	Noise.SetNoiseType(FastNoiseLite::NoiseType_Perlin);
	Noise.SetSeed(SeedMoisture);

	for (int X = 0; X < Dimension; X++)
	{
		for (int Y = 0; Y < Dimension; Y++)
		{
			MoistureMap.Add(FVector2D((float)X, (float)Y), Noise.GetNoise((float)X, (float)Y));
		}
	}
	return MoistureMap;
}

NoiseMapGenerator::~NoiseMapGenerator()
{
}
