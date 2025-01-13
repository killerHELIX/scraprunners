// Fill out your copyright notice in the Description page of Project Settings.


#include "DesertGenerator.h"
#include "Kismet/KismetMathLibrary.h"

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
	GenerateDesert();
}

// Called every frame
void ADesertGenerator::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ADesertGenerator::GenerateDesert()
{
	TArray<FVector> Vertices;
	TArray<int32> Triangles;
	TArray<FVector> Normals;
	TArray<FVector2D> UVs;
	TArray<FProcMeshTangent> Tangents;
	TArray<FColor> VertexColors;

	// make grid
	for (int32 x = 0; x <= Width; x++)
	{
		for (int32 y = 0; y <= Length; y++)
		{
			float Height = FMath::PerlinNoise2D(FVector2D(x, y) * NoiseFrequency) * HeightMultiplier;
			Vertices.Add(FVector(x * Scale, y * Scale, Height));
			Normals.Add(FVector(0, 0, 1));
			UVs.Add(FVector2D((float)x / Width, (float)y / Length));

			int32 BottomLeft = x * (Length + 1) + y;
			int32 BottomRight = BottomLeft + 1;
			int32 TopLeft = BottomLeft + (Length + 1);
			int32 TopRight = TopLeft + 1;
			
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

	for (int32 i = 0; i < Vertices.Num(); i++)
	{
		Tangents.Add(FProcMeshTangent(1.f, 0.f, 0.f));
		VertexColors.Add(FColor::White);
	}

	Mesh->CreateMeshSection(0, Vertices, Triangles, Normals, UVs, VertexColors, Tangents, true);

	Mesh->ContainsPhysicsTriMeshData(true);
}

