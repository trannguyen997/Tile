TiledPlugin {
    cpp.defines: base.concat(["FLARE_LIBRARY"])

    files: [
        "flare_global.h",
        "flareplugin.cpp",
        "flareplugin.h",
        "plugin.json",
    ]
}
