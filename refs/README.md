# refs/ — assemblies de referência locais (NÃO versionadas)

O `Nautilus.dll` **não está no nuget.org**. ⚠️ O pacote chamado `Nautilus` lá é o
*OctopusDeploy-Nautilus*, projeto sem relação nenhuma com Subnautica — referenciá-lo
compila e depois quebra.

Coloque aqui o `Nautilus.dll` **da mesma versão que roda no seu jogo**. Duas formas:

**1. Copiar da instalação** (mais simples, garante a versão exata):
```
copy "%SUBNAUTICA_GAME_DIR%\BepInEx\plugins\Nautilus\Nautilus.dll" refs\
```

**2. Compilar da fonte** (quando precisar de uma versão específica):
```
git clone https://github.com/SubnauticaModding/Nautilus
dotnet build Nautilus/Nautilus.csproj -c SN.STABLE     # exige .NET SDK 10+ (usa C# 14)
```

O build também aceita o caminho por variável de ambiente, sem copiar nada:
```
set NAUTILUS_DLL=C:\...\BepInEx\plugins\Nautilus\Nautilus.dll
```

Versão confirmada em uso: **1.0.0-pre.53** (a mesma do `Version.targets` no master
do Nautilus — ou seja, a instalação do operador está atual).
