# Créditos e licença — Subnautica Unhinged

## Licença: GPL-3.0-or-later

O projeto é **GPL-3.0-or-later**, e essa escolha não foi de gosto — foi imposta pela
pilha em que ele se apoia:

- **Nautilus** (SubnauticaModding) — GPL-3.0
- **VehicleFramework** (NeisesMike) — GPL-3.0

O `Unhinged.Legacy.dll` faz *link* com o Nautilus. Distribuir um binário derivado de
código GPL-3.0 obriga o conjunto a ser GPL-3.0 compatível. Não havia terceira opção:
ou o projeto é GPL-3.0, ou não pode usar o Nautilus — e sem o Nautilus não existe
ponte para o jogo moderno.

O texto integral está em [`LICENSE`](LICENSE), copiado de
<https://www.gnu.org/licenses/gpl-3.0.txt>.

## O que este pacote contém

**Só código deste projeto.** Nenhum arquivo de terceiro é redistribuído aqui:

| Arquivo | Origem |
| --- | --- |
| `Unhinged.Core.dll` | Escrito para este projeto |
| `Unhinged.Legacy.dll` | Escrito para este projeto |

## De quem depende (não incluído — você já tem instalado)

| Projeto | Autor | Licença |
| --- | --- | --- |
| [BepInEx 5](https://github.com/BepInEx/BepInEx) | BepInEx | LGPL-2.1 |
| [Nautilus](https://github.com/SubnauticaModding/Nautilus) | SubnauticaModding | GPL-3.0 |
| [HarmonyX](https://github.com/BepInEx/HarmonyX) | BepInEx | MIT |
| [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) | James Newton-King | MIT — empacotado **pelo jogo** |

*Subnautica* é da **Unknown Worlds Entertainment**. Este projeto não é afiliado a eles,
e nenhum arquivo do jogo é redistribuído.

## Autoria do que ainda vai ser portado

A ponte `Unhinged.Legacy` existe para portar mods de terceiros. **Nada disso está
incluído nesta versão**, mas quando estiver, vale o que já foi verificado e registrado
em [`docs/SOURCES.md`](docs/SOURCES.md) e [`docs/LICENCAS-E-FUSAO.md`](docs/LICENCAS-E-FUSAO.md):

- **Field Creators Studios** (ccgould) — MIT. É a maior fonte prevista; MIT permite
  portar, fundir e redistribuir **com atribuição**.
- Mods **sem licença declarada** (Socknautica/S.O.C.K. Tank, SealSub, PrimeSonic,
  Console Improved) ficam **de fora** até autorização escrita do autor. Sem licença
  não significa livre — significa todos os direitos reservados.

Nenhum código ou asset de terceiro é apresentado aqui como criação própria.
