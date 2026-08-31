using System;
using System.Collections;
using FCS_AlterraHub.Systems;
using FCSCommon.Utilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FCS_AlterraHub.Patches
{
    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.Start))]
    public class EndCreditsManager_Start_Patch
    {
        /// <summary>
        /// Quanto tempo os créditos levam para rolar até o fim.
        ///
        /// ⚠️ VALOR A CALIBRAR EM JOGO. O original lia
        /// <c>EndCreditsManager.secondsUntilScrollComplete</c>, campo que **não existe
        /// mais**: o jogo reescreveu a classe inteira — o texto em três colunas
        /// (<c>leftText</c>/<c>centerText</c>/<c>rightText</c>) virou um
        /// <c>textField</c> só, e a rolagem virou baseada em fases
        /// (<c>phase</c>, <c>scrollSpeed</c>, <c>contentHeight</c>).
        ///
        /// A duração equivalente **não é derivável** dos campos novos no <c>Start</c>:
        /// <c>contentHeight</c> só é preenchido depois do layout. Deixar um número
        /// explícito e nomeado é honesto; inventar uma fórmula com cara de precisa
        /// seria pior. Se a fala entrar cedo ou tarde demais, é este número que muda.
        /// </summary>
        internal const float CreditsScrollSeconds = 200f;

        // ─────────────────────────────────────────────────────────────────────────
        // PORTE — de Prefix para Postfix.
        //
        // O original era um **Prefix que reimplementava o `Start()` da vanilla**: mexia
        // em `fadeLogo`, `startFadeTime`, `goToPos`, trocava os textos de PS4 e chamava
        // `PlayMusic`, para então devolver `false` e impedir o original de rodar.
        //
        // Nenhum desses campos existe hoje. Reimplementar um `Start()` que foi reescrito
        // seria adivinhar; e a parte do FCS que interessa não é a rolagem — é a fala de
        // fim conforme a dívida. Como Postfix, a vanilla cuida da própria animação e o
        // FCS só acrescenta o que é dele. Menos superfície, mesmo efeito.
        // ─────────────────────────────────────────────────────────────────────────
        [HarmonyPostfix]
        public static void Postfix(EndCreditsManager __instance)
        {
            try
            {
                if (__instance == null || CardSystem.main == null) return;

                string key = null;
                if (CardSystem.main.IsDebitPaid()) key = "Play_DebtPaid";
                else if (CardSystem.main.HasPaymentBeenMadeToDebit()) key = "Play_NotDebtPaid";

                // Sem dívida quitada nem pagamento parcial: créditos vanilla, sem fala.
                if (key == null) return;

                __instance.StartCoroutine(ReturnToMainMenu(CreditsScrollSeconds, key));
            }
            catch (Exception e)
            {
                // Um patch de créditos não pode derrubar o fim da história.
                QuickLogger.Error(e.Message);
                QuickLogger.Error(e.StackTrace);
                QuickLogger.Info("Falha ao aplicar o patch do EndCreditsManager; créditos seguem os da vanilla.");
            }
        }

        public static IEnumerator ReturnToMainMenu(float seconds, string key)
        {
            yield return new WaitForSeconds(seconds - 3f);
            QuickLogger.Debug("Debt has been paid Skipping", true);
            VoiceNotificationSystem.main.Play($"{key}_key");
            yield return new WaitForSeconds(10.5f);
            yield return SceneManager.LoadSceneAsync("Cleaner", LoadSceneMode.Single);
            yield break;
        }
    }
}