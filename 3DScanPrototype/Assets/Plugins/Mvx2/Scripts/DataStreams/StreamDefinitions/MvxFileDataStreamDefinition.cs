﻿using UnityEngine;

namespace MVXUnity
{
    //[CreateAssetMenu(fileName = "FileDataStreamDefinition", menuName = "Mvx2/Data Stream Definitions/File Data Stream Definition")]
    public class MvxFileDataStreamDefinition : MvxDataStreamDefinition
    {
        #region data

        /// <summary> Path to MVX file. Can be full path or relative to StreamingAssets folder. </summary>
        [SerializeField, HideInInspector] private string m_filePath;
        public string filePath
        {
            get { return m_filePath; }
            set
            {
                if (m_filePath == value)
                    return;
                m_filePath = value;
                m_fileAccessor.Reset();

                onDefinitionChanged.Invoke();
            }
        }

        private MvxFileAccessor m_fileAccessor = new MvxFileAccessor();
        private string GetFilePath()
        {
            return m_fileAccessor.PrepareFile(m_filePath);
        }

        #endregion

        public override MvxDataStreamSourceRuntime AppendSource(Mvx2API.ManualGraphBuilder graphBuilder)
        {
            string mvxFilePath = GetFilePath();
            graphBuilder.AppendGraphNode(new Mvx2BasicIO.Mvx2FileReaderGraphNode(mvxFilePath));
            return new MvxDataStreamSourceRuntime();
        }
    }
}
