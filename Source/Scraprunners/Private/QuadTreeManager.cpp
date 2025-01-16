// Fill out your copyright notice in the Description page of Project Settings.


#include "QuadTreeManager.h"
#include "NoiseMapGenerator.h"

// Sets default values
AQuadTreeManager::AQuadTreeManager()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AQuadTreeManager::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AQuadTreeManager::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void AQuadTreeManager::InitializeQuadtree(FVector2D WorldMin, FVector2D WorldMax)
{
	RootNode = new FQuadtreeNode(WorldMin, WorldMax, 0);
	SubdivideNode(RootNode);
}

void AQuadTreeManager::SubdivideNode(FQuadtreeNode* Node)
{
	FVector2D Min = Node->BoundsMin;
	FVector2D Max = Node->BoundsMax;
	FVector2D Mid = (Min + Max) / 2;

	if ((Max - Min).Size() <= MinNodeSize)
	{
		return;
	}

	Node->bIsLeaf = false;
	Node->Children.SetNum(4);

	Node->Children[0] = new FQuadtreeNode(Min, Mid, Node->LODLevel + 1); //Top Left
	Node->Children[1] = new FQuadtreeNode(FVector2D(Mid.X, Min.Y), FVector2D(Max.X, Mid.Y), Node->LODLevel + 1); //Top Right
	Node->Children[2] = new FQuadtreeNode(FVector2D(Min.X, Mid.Y), FVector2D(Mid.X, Max.Y), Node->LODLevel + 1); //Bottom Left
	Node->Children[3] = new FQuadtreeNode(Mid, Max, Node->LODLevel + 1); //Bottom Right

	for (auto* Child : Node->Children)
	{
		SubdivideNode(Child);
	}
}

void AQuadTreeManager::UpdateLOD(FQuadtreeNode* Node)
{
	if (!Node || Node->bIsLeaf)
	{
		return;
	}
	float Distance = GetDistanceToPlayer(Node);

	if (Distance < HighDetailThreshold)
	{
		Node->LODLevel = 0;
	}
	else if (Distance < 2 * HighDetailThreshold)
	{
		Node->LODLevel = 1;
	}
	else
	{
		Node->LODLevel = 2;
	}

	for (auto* Child : Node->Children)
	{
		UpdateLOD(Child);
	}
}

float AQuadTreeManager::GetDistanceToPlayer(FQuadtreeNode* Node)
{
	FVector2D Center = (Node->BoundsMax + Node->BoundsMin) / 2;
	FVector2D PlayerLocation2D(Player->GetActorLocation().X, Player->GetActorLocation().Y);
	return FVector2D::Distance(Center, PlayerLocation2D);
}

void AQuadTreeManager::DestroyQuadtree(FQuadtreeNode* Node)
{
	if (!Node) return;

	for (auto* Child : Node->Children)
	{
		DestroyQuadtree(Child);
	}

	delete Node;
}