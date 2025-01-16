// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "FQuadtreeNode.generated.h"

USTRUCT()
struct FQuadtreeNode
{
	GENERATED_BODY()

	FVector2D BoundsMin;
	FVector2D BoundsMax;
	TArray<FQuadtreeNode*> Children;
	int32 LODLevel;
	bool bIsLeaf;

	FQuadtreeNode() : LODLevel(0), bIsLeaf(true) {}

	FQuadtreeNode(FVector2D InMin, FVector2D InMax, int32 InLODLevel ) : BoundsMin(InMin), BoundsMax(InMax), LODLevel(InLODLevel), bIsLeaf(true) {}
};