TiledPlugin {
    cpp.defines: base.concat(["GMX_LIBRARY"])

    files: [
        "gmx_global.h",
        "gmxplugin.cpp",
        "gmxplugin.h",
        "plugin.json",
    ]
}
