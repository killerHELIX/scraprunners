// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "ProceduralMeshComponent.h"
#include "DesertGenerator.generated.h"

UCLASS()
class SCRAPRUNNERS_API ADesertGenerator : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ADesertGenerator();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UPROPERTY(VisibleAnywhere)
	UProceduralMeshComponent* Mesh;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	int32 Seed = 1;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float ScalarForPerlinVector2 = 0.05f;

	// Parameters

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	int32 GridSize = 16;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	int32 ChunkSize = 32;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	int32 Width = 512;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	int32 Length = 512;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float Scale = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float HeightMultiplier = 200.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float NoiseFrequency = 0.01f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float LOD0Distance = 1000.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float LOD1Distance = 3000.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Desert Terrain")
	float LOD2Distance = 5000.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Materials")
	float TilingFactor = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Materials")
	UMaterialInterface* SandMaterial;

	void GenerateChunk(FVector ChunkCenter, int32 LODLevel);
	void UpdateLOD(FVector PlayerPosition);

private:
	TMap<FVector, int32> ChunkLODMap;
	FVector LastPlayerPosition;
};
