# FC Studios — compatibilidade de save

> ## ⚠️ NÃO VERIFICADO
>
> **Nenhum** dos cenários abaixo foi testado. Não há nada aqui que possa ser
> chamado de compatível ou incompatível — só o que precisa ser testado e por quê.
>
> A regra do projeto é explícita: **nunca declarar compatibilidade sem teste.**

## Cenários a testar

| # | cenário | o que observar | resultado |
| --- | --- | --- | --- |
| 1 | **save novo** | criar mundo, construir item do FCS, salvar, sair, carregar | ⬜ |
| 2 | **save existente sem FCS** | carregar save antigo com o mod instalado — não pode quebrar | ⬜ |
| 3 | **save com conteúdo do FCS original** | save feito com o FCS **legado** (QMod/SMLHelper) — os itens sobrevivem? | ⬜ |
| 4 | **remoção** | desinstalar o mod e carregar um save que tinha itens dele | ⬜ |
| 5 | **atualização** | trocar 1.1.0 por uma versão futura sobre o mesmo save | ⬜ |
| 6 | **módulo desligado** | `EnableStorage=false` num save que tem Alterra Storage construído | ⬜ |

## Por que o cenário 3 é o mais arriscado

O FCS legado salvava com o `ModUtils.Save`/`LoadSaveData` do SMLHelper. A ponte
**não implementa** esses dois — estão marcados `REQUIRES_RUNTIME_TEST` de
propósito:

> Mexe em **dado de save**. Errar aqui corrompe o save de quem joga, então merece
> verificação própria antes de ser escrito — não vale deduzir.

Enquanto não forem implementados, um save do FCS legado provavelmente **não**
recupera o estado dos dispositivos (conteúdo de armazenamento, saldo, ligações).
Os itens podem existir e voltar vazios. Isso é hipótese, não medição.

## Por que o cenário 6 merece cuidado

Desligar um módulo faz os `[QModCore]` dele não rodarem, então os TechTypes não
são registrados. Um save que tem esse item construído vai encontrar um prefab
que não existe mais. O comportamento do Subnautica nesse caso é conhecido por
ser silencioso — o objeto some — mas **não testamos**.

## Recomendação até haver teste

**Faça backup do save antes de instalar.** Não é fórmula de praxe: os cenários 3
e 6 são plausíveis o suficiente para justificar a cópia, e ninguém aqui pode
afirmar o contrário com evidência.

Pasta dos saves:
`%USERPROFILE%\AppData\LocalLow\Unknown Worlds\Subnautica\Subnautica\SavedGames\`

## Como preencher esta tabela

Cada cenário: executar, coletar `BepInEx\LogOutput.log`, e registrar o resultado
com a evidência. Um ⬜ que vira ✅ sem log anexo não vale mais que o ⬜.
