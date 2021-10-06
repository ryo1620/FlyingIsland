#ifdef __cplusplus
extern "C" {
#endif
    // C#から呼ばれる関数
    // Retinaの倍率を取得する
    float getRetinaScale() {
        return UIScreen.mainScreen.scale;
    }
#ifdef __cplusplus
}
#endif
