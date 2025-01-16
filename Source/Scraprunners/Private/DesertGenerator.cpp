// Fill out your copyright notice in the Description page of Project Settings.


#include "DesertGenerator.h"
#include "Kismet/KismetMathLibrary.h"
#include "Kismet/GameplayStatics.h"
#include "FastNoiseLite.h"

// Sets default values
ADesertGenerator::ADesertGenerator()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = false;

	Mesh = CreateDefaultSubobject<UProceduralMeshComponent>(TEXT("ProceduralMesh"));
	RootComponent = Mesh;
}

// Called when the game starts or when spawned
void ADesertGenerator::BeginPlay()
{
	Super::BeginPlay();
	
	Seed = FMath::Rand();
	LastPlayerPosition = FVector::ZeroVector;

	if (SandMaterial) {
		Mesh->SetMaterial(0, SandMaterial);
	}

	for (int X = 0; X < GridSize; X++)
	{
		for (int Y = 0; Y < GridSize; Y++)
		{
			FVector ChunkCenter = FVector(X * ChunkSize * Scale, Y * ChunkSize * Scale, 0);
			int32 InitialLOD = 2;
			ChunkLODMap.Add(ChunkCenter, InitialLOD);
			GenerateChunk(ChunkCenter, InitialLOD);
		}
	}
}

// Called every frame
void ADesertGenerator::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	
	//Get player position
	APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	if (PlayerController)
	{
		APawn* PlayerPawn = PlayerController->GetPawn();
		if (PlayerPawn)
		{
			FVector PlayerPosition = PlayerPawn->GetActorLocation();

			if (FVector::Dist(PlayerPosition, LastPlayerPosition) > ChunkSize)
			{
				UpdateLOD(PlayerPosition);
				LastPlayerPosition = PlayerPosition;
			}
		}
	}
}

void ADesertGenerator::GenerateChunk(FVector ChunkCenter, int32 LODLevel)
{
	//Clear previous meshes
	Mesh->ClearAllMeshSections();

	FastNoiseLite Noise;
	Noise.SetNoiseType(FastNoiseLite::NoiseType_Perlin);
	Noise.SetSeed(Seed);

	int32 Resolution = ChunkSize / pow(2, LODLevel);

	FRandomStream RandomStream(Seed);
	TArray<FVector> Vertices;
	TArray<int32> Triangles;
	TArray<FVector> Normals;
	TArray<FVector2D> UVs;
	TArray<FProcMeshTangent> Tangents;
	TArray<FColor> VertexColors;

	// make grid
	for (int32 x = 0; x <= Resolution; x++)
	{
		for (int32 y = 0; y <= Resolution; y++)
		{
			float Height = Noise.GetNoise((float)x, (float)y) * HeightMultiplier;
			Vertices.Add(FVector(x * Scale, y * Scale, Height) + ChunkCenter);
			Normals.Add(FVector(0, 0, 1));
			UVs.Add(FVector2D((float)x / Width * TilingFactor / Resolution, (float)y / Length * TilingFactor / Resolution));

			int32 BottomLeft = x * (Resolution + 1) + y;
			int32 BottomRight = BottomLeft + 1;
			int32 TopLeft = BottomLeft + (Resolution + 1);
			int32 TopRight = TopLeft + 1;
			
			if (y < Length && x < Width)
			{
				//1st tri
				Triangles.Add(BottomLeft);
				Triangles.Add(BottomRight);
				Triangles.Add(TopRight);

				//2nd tri
				Triangles.Add(BottomLeft);
				Triangles.Add(TopRight);
				Triangles.Add(TopLeft);
			}
		}
	}

	for (int32 i = 0; i < Vertices.Num(); i++)
	{
		Tangents.Add(FProcMeshTangent(1.f, 0.f, 0.f));
		VertexColors.Add(FColor::White);
	}

	Mesh->CreateMeshSection(0, Vertices, Triangles, Normals, UVs, VertexColors, Tangents, true);

	Mesh->ContainsPhysicsTriMeshData(true);
}

void ADesertGenerator::UpdateLOD(FVector PlayerPosition)
{
	for (auto& Entry : ChunkLODMap)
	{
		FVector ChunkCenter = Entry.Key;
		float Distance = FVector::Dist(PlayerPosition, ChunkCenter);

		int32 NewLODLevel = 0;
		if (Distance < LOD0Distance)
		{
			NewLODLevel = 0;
		}
		else if (Distance < LOD1Distance)
		{
			NewLODLevel = 1;
		}
		else
		{
			NewLODLevel = 2;
		}

		if (Entry.Value != NewLODLevel)
		{
			Entry.Value = NewLODLevel;
			GenerateChunk(ChunkCenter, NewLODLevel);
		}
	}
}