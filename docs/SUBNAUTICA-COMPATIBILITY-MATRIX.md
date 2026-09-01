# Matriz de compatibilidade

> ## ⚠️ A build INSTALADA não foi verificada
>
> O adendo é explícito: *"não considerar o `Subnautica.GameLibs` presente no
> `.csproj` como automaticamente equivalente à build instalada"*. **Não considero.**
> Este ambiente é **Linux, sem Subnautica** — não há instalação para consultar.
>
> O que sei é o que o **artefato** diz. O que a **máquina do operador** tem continua
> desconhecido, e é a única divergência que resta para o porte poder ser chamado
> de concluído.

| Componente | Versão | Evidência | Estado |
| --- | --- | --- | --- |
| **Subnautica (instalado)** | **?** | — nenhuma | ❌ **UNVERIFIED** |
| Subnautica (alvo) | 82304 | versão do pacote `Subnautica.GameLibs`; é a mais nova das 4 publicadas no feed do BepInEx | ⚠️ **ASSUMED** |
| GameLibs | 82304.0.0-r.0 | `<PackageReference>` no `.csproj`; DLLs presentes no cache e usadas na compilação | ✅ VERIFIED |
| Unity | 2019.4.36 | `<PackageReference Include="UnityEngine.Modules">`; **referência**, não runtime | ⚠️ **ASSUMED** |
| BepInEx | 5.4.21 | `<PackageReference Include="BepInEx.Core">` — referência de compilação | ⚠️ **ASSUMED** |
| HarmonyX | 2.7.0 | dependência declarada pelo `BepInEx.Core` 5.4.21 | ✅ VERIFIED |
| Nautilus | 1.0.0-pre.53 | string dentro do `Nautilus.dll` real usado como referência; versão de assembly `1.0.0.53` | ✅ VERIFIED |
| Target Framework | net472 | `<TargetFramework>`; mesmo `v4.7.2` dos `.csproj` do upstream | ✅ VERIFIED |

## O que cada estado significa aqui

- **VERIFIED** — medido num artefato que eu abri: metadata de DLL, conteúdo de
  pacote, saída de build. Não é leitura de documentação.
- **ASSUMED** — vem de uma declaração de projeto ou de convenção de feed, e
  **não** foi confrontado com o jogo instalado. Compilar contra `net472` prova
  que o compilador aceitou; não prova qual runtime vai carregar o DLL.
- **UNVERIFIED** — não há evidência nenhuma.

⚠️ A build 82304 é `ASSUMED` por um motivo concreto: **o número não está gravado
dentro da `Assembly-CSharp.dll`** — procurei. Ele vem do versionamento do pacote,
que é a convenção do feed do BepInEx. É uma boa evidência; não é a mesma coisa que
ler a versão do jogo instalado.

## A divergência a resolver

Se a build instalada **não** for a 82304, o que este porte garante muda de figura:

| se a instalada for… | consequência |
| --- | --- |
| **82304** | os 66 alvos de patch e os 6 alvos de reflexão conferidos valem para ela |
| **71288 ou anterior** (série antiga) | o porte pode não carregar: os alvos foram conferidos contra a 82304, e o `Oculus.Newtonsoft.Json` do `#if SUBNAUTICA_STABLE` volta a ser o namespace certo — ou seja, estaríamos compilando o lado errado do `#if` |
| **mais nova que 82304** | não há GameLibs para ela no feed; alvo de patch pode ter sido renomeado sem que nada aqui perceba |

**Como resolver, do lado do operador** — qualquer um destes basta:

1. Steam → Subnautica → Propriedades → Betas: qual branch está marcada.
2. `BepInEx\LogOutput.log`, primeiras linhas: o BepInEx registra a versão do
   Unity e do jogo no cabeçalho.
3. O `BepInEx\Unhinged-Relatorio.md` que o `Unhinged.Core` escreve a cada partida
   — ele coleta exatamente isso.

Com esse número, esta matriz sai de `ASSUMED` para `VERIFIED` ou aponta um
conflito real. **Enquanto isso não acontecer, o porte não pode ser chamado de
concluído** — é o próprio critério do adendo.
