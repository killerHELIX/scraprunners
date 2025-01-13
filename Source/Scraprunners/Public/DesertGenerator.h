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

	// Parameters
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

	void GenerateDesert();
};
