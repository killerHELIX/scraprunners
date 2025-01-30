// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "Biome.generated.h"

/**
 * 
 */
UCLASS()
class SCRAPRUNNERS_API UBiome : public UObject
{
	GENERATED_BODY()
	
public:
	UBiome();

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	int Seed = 1;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	int Width;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	int Length;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	float Height;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	float Scale;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	float NoiseFrequency;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	float HeightMultiplier;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Biome");
	float ScalarForNoise;

	TArray<FVector2D> VertexPositions;
	TArray<FVector2D> GeneratePositionsForVertices();
};
