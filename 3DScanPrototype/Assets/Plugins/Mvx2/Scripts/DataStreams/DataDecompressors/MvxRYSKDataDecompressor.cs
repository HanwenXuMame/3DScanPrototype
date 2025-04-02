using UnityEngine;

namespace MVXUnity
{
    /// <summary>
    /// A wrapper for RYSK decompressor.
    /// </summary>
    /// <remarks>
    /// Picks one of RYSK data layers in the stream and decompresses it.
    /// </remarks>
    [CreateAssetMenu(fileName = "RYSKDataDecompressor", menuName = "Mvx2/Data Decompressors/RYSK Data Decompressor")]
    public class MvxRYSKDataDecompressor : MvxDataDecompressor
    {
        #region data

        const string DROP_DECODED_LAYERS = "Drop decoded layers";
        
        [Tooltip("Indicates whether original compressed data shall be dropped from the frame after decompression")]
        public bool dropCompressedInput = true;

        #endregion

        public override void AppendDecompressor(Mvx2API.ManualGraphBuilder graphBuilder)
        {
            Mvx2API.SingleFilterGraphNode ryskDecompressor = new Mvx2API.SingleFilterGraphNode(MVCommon.Guid.FromHexString("A69C9330-C83A-4439-83D3-C3F470865B02"), true);

            ryskDecompressor.SetFilterParameterValue(DROP_DECODED_LAYERS, dropCompressedInput ? "true" : "false");

            graphBuilder = graphBuilder + ryskDecompressor;
        }
    }
}
