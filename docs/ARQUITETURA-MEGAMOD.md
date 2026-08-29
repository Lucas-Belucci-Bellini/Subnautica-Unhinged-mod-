# O mega mod — como juntar 75 mods sem juntar 75 DLLs

## O que você pediu

> "juntar cada um desses que estão separados e fazer um mega mod que eu possa mexer sem
> medo (…) até o momento eu só tenho que seguir as limitações dos mods, mas eu quero fazer
> loucuras com os mods já instalados, e também adicionar mais coisa"

Três desejos, e vale separá-los porque **só um deles é problemático**:

1. **Passar por cima dos limites dos mods** — ✅ totalmente viável
2. **Fazer os mods conversarem entre si** — ✅ totalmente viável
3. **Adicionar coisa nova** — ✅ totalmente viável
4. *Fundir os binários num só* — ⛔ é o único item ruim, e **é desnecessário para 1–3**

## Por que fundir os binários é o caminho errado

Não é conservadorismo — é que fundir **piora** exatamente o que você quer:

- **Congela tudo.** Uma cópia do Vehicle Framework 2.0.8 dentro do seu DLL não recebe a
  2.0.9. Você troca "seguir a limitação do mod" por "manter 75 forks para sempre".
- **Quebra a lei e as regras do próprio projeto.** A maioria desses mods é de terceiros,
  vários sem fonte pública. O `PROJECT_CONTEXT.md` já decidiu: *"não distribuir DLLs
  originais como se fossem código próprio"*, *"preservar autores, licenças e permissões"*.
  E o Nautilus é **GPL-3.0-only**.
- **Não é mais poder.** Fundir dá acesso ao código. Mas patch em runtime **também** dá — e
  sem carregar o peso.

## O caminho: camada de override

O Unhinged não vira um saco com 75 mods dentro. Ele vira **a camada que manda neles**.

```
   BepInEx carrega os 75 mods normalmente (continuam atualizáveis pelo Vortex)
                              │
                              ▼
   Unhinged.Core carrega DEPOIS de todos e alcança cada um em runtime
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
   reescreve a          patcha o código        adiciona sistemas
   config deles          deles (Harmony)          novos por cima
```

Você não segue mais a limitação do mod: **você a reescreve, de fora, sem tocar no mod.**

## Que isto é real — verificado, não suposto

Todas as APIs abaixo foram lidas do metadata de `BepInEx.dll` 5.4.x e `0Harmony.dll` 2.7.0:

| Capacidade | API confirmada |
| --- | --- |
| Listar todo mod carregado | `Chainloader.PluginInfos` → `Dictionary<string, PluginInfo>` ✅ |
| Ver os que **falharam** ao carregar | `Chainloader.DependencyErrors` → `List<string>` ✅ |
| Pegar a **instância viva** de outro mod | `PluginInfo.Instance` → `BaseUnityPlugin` ✅ |
| Chegar na config dele | `BaseUnityPlugin.Config` → `ConfigFile` ✅ |
| **Ler e reescrever** uma entrada dele | `ConfigFile.TryGetEntry<T>(section, key, out ConfigEntry<T>)` ✅ |
| Achar um tipo dele sem referenciá-lo | `AccessTools.TypeByName(string)` ✅ |
| Achar um método dele para patchar | `AccessTools.Method(Type, string, Type[])` ✅ |
| Carregar depois dele, sem exigir que exista | `BepInDependency(guid, SoftDependency)` ✅ |

O encadeamento inteiro, numa linha:

```csharp
Chainloader.PluginInfos["guid.do.mod"].Instance.Config
    .TryGetEntry<float>("Secao", "Chave", out var e);
e.Value = 99999f;   // o teto que a UI dele recusaria
```

**`AcceptableValueRange` só é aplicado na borda de entrada** (UI/arquivo), não na
atribuição. Escrever pela API passa por cima do limite — e é por isso que isto funciona.

## As duas formas de limite, e o antídoto de cada

| Onde o mod trava o valor | Como o Unhinged passa por cima |
| --- | --- |
| Na **config** (a maioria) | `ModBridge.TrySetConfig(...)` — reescreve além do teto |
| No **código** (`const`, `if (x > 100)`) | Patch do Harmony no método, via `ModBridge.FindMethod(...)` |

O segundo caso não é hipotético: a nota do scanner já mostrou que
`MapRoomFunctionality.defaultRange` é `const` e **não pode** ser patchada — só o método
que a lê. A mesma lógica vale para os limites dos mods.

⚠️ **Ressalva honesta:** reescrever a config só faz efeito se o mod *lê* aquele valor
quando usa. Mod que lê uma vez no carregamento e guarda o resultado exige patch no lugar.
Por isso `TrySetConfig` retorna se a **escrita** ocorreu — não promete que o mod obedeceu.
Isso se descobre testando, caso a caso.

## O que já está construído

`src/Unhinged.Core/Interop/`:

- **`ModRegistry`** — inventário em runtime: `All`, `IsLoaded(guid)`, `GetVersion(guid)`,
  `FindByName(fragmento)` (útil porque muitos mods do Nexus não publicam o GUID) e
  `LoadFailures`.
- **`ModBridge`** — `TryGetConfig` / `TrySetConfig` / `FindType` / `FindMethod`. Tudo
  tolerante a falha: retorna `false` e loga, nunca derruba o jogo porque um mod opcional
  mudou de forma.

Ligado no `Awake`: **já no primeiro boot o log lista os mods carregados e, em nível
Warning, os que falharam.** Isso sozinho já deve mostrar o estado real da camada legada
(QModManager/SMLHelper) sem adivinhação.

## Ordem de construção sugerida

1. **Rodar uma vez e ler o log.** Sai o inventário real com **GUIDs** — que é o que falta
   para endereçar cada mod. Hoje temos nomes do Vortex, não GUIDs.
2. **Mapear GUID → capacidade** num catálogo (`docs/MOD_REGISTRY.md`), a partir desse log.
3. **Primeiro override real**, pequeno e reversível: subir um teto de um mod pela config e
   confirmar em jogo que pegou.
4. **Integração**: fazer dois mods conversarem (ex.: alcance do scanner ↔ Sonar Module).
5. **Conteúdo novo** por cima — aí sim com Nautilus, e com a versão dele já confirmada.

Cada override entra atrás de uma chave de config e **pode ser desligado**, conforme a regra
do `PLAN.md`: *"cada correção deve indicar qual mod afeta, por que é necessária e como pode
ser desativada"*.

## O limite que permanece

Isto tudo funciona **na sua máquina, sobre os mods que você já instalou**. Distribuir o
Unhinged para outras pessoas continua exigindo que elas instalem os mods originais — e é
assim que os créditos e as licenças ficam preservados. O Unhinged é a camada que faz os
75 obedecerem; não é o pacote que os contém.
