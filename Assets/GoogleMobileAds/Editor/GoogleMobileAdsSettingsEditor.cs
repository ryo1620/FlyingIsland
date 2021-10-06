using UnityEditor;

namespace GoogleMobileAds.Editor
{
    [InitializeOnLoad]
    [CustomEditor(typeof(GoogleMobileAdsSettings))]
    public class GoogleMobileAdsSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty m_Android;
        SerializedProperty m_IOS;
        SerializedProperty m_DelayAppMeasurementInit;
        [MenuItem("Assets/Google Mobile Ads/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = GoogleMobileAdsSettings.Instance;
        }
        private void OnEnable()
        {
            m_Android = serializedObject.FindProperty("adMobAndroidAppId");
            m_IOS = serializedObject.FindProperty("adMobIOSAppId");
            m_DelayAppMeasurementInit = serializedObject.FindProperty("delayAppMeasurementInit");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Android);
            EditorGUILayout.PropertyField(m_IOS);
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Google Mobile  Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713", MessageType.Info);
            EditorGUILayout.Space(15);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(m_DelayAppMeasurementInit);
            if (m_DelayAppMeasurementInit.boolValue)
                EditorGUILayout.HelpBox("When DelayAppMeasurementInit is on it delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.", MessageType.Info);
            if (serializedObject.hasModifiedProperties)
            {
                GoogleMobileAdsSettings.Instance.GoogleMobileAdsAndroidAppId = m_Android.stringValue.Trim();
                GoogleMobileAdsSettings.Instance.GoogleMobileAdsIOSAppId = m_IOS.stringValue.Trim();
                GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit = m_DelayAppMeasurementInit.boolValue;
                GoogleMobileAdsSettings.Instance.WriteSettingsToFile();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty((GoogleMobileAdsSettings)target);
            }
        }
    }
}