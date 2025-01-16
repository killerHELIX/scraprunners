// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "FQuadtreeNode.h"
#include "QuadTreeManager.generated.h"

UCLASS()
class SCRAPRUNNERS_API AQuadTreeManager : public AActor
{
	GENERATED_BODY()
	
public:	

	FQuadtreeNode* RootNode;

	AActor* Player;

	//params
	int32 MaxLODLevel;
	float HighDetailThreshold;
	float MinNodeSize;
	// Sets default values for this actor's properties
	AQuadTreeManager();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	void InitializeQuadtree(FVector2D WorldMin, FVector2D WorldMax);

	void SubdivideNode(FQuadtreeNode* Node);

	void UpdateLOD(FQuadtreeNode* Node);

	float GetDistanceToPlayer(FQuadtreeNode* node);

	void DestroyQuadtree(FQuadtreeNode* Node);

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
