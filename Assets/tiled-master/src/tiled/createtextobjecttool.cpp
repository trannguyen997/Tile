/*
 * createtextobjecttool.cpp
 * Copyright 2017, Thorbjørn Lindeijer <bjorn@lindeijer.nl>
 *
 * This file is part of Tiled.
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */

#include "createtextobjecttool.h"

#include "mapdocument.h"
#include "mapobject.h"
#include "mapobjectitem.h"
#include "maprenderer.h"
#include "objectgroup.h"
#include "snaphelper.h"
#include "utils.h"

namespace Tiled {

CreateTextObjectTool::CreateTextObjectTool(QObject *parent)
    : CreateObjectTool("CreateTextObjectTool", parent)
{
    QIcon icon(QLatin1String(":images/24/insert-text.png"));
    icon.addFile(QLatin1String(":images/48/insert-text.png"));
    setIcon(icon);
    setShortcut(Qt::Key_E);
    Utils::setThemeIcon(this, "insert-text");
    languageChangedImpl();
}

void CreateTextObjectTool::mouseMovedWhileCreatingObject(const QPointF &pos, Qt::KeyboardModifiers modifiers)
{
    MapObject *newMapObject = mNewMapObjectItem->mapObject();
    const QPointF halfSize(newMapObject->width() / 2, newMapObject->height() / 2);
    const QRectF screenBounds { pos - halfSize, newMapObject->size() };

    // These screenBounds assume TopLeft alignment, but the map's object alignment might be different.
    const QPointF offset = alignmentOffset(screenBounds, newMapObject->alignment(mapDocument()->map()));

    const MapRenderer *renderer = mapDocument()->renderer();
    QPointF pixelCoords = renderer->screenToPixelCoords(screenBounds.topLeft() + offset);

    SnapHelper(renderer, modifiers).snap(pixelCoords);

    newMapObject->setPosition(pixelCoords);
    mNewMapObjectItem->syncWithMapObject();
}

void CreateTextObjectTool::languageChanged()
{
    CreateObjectTool::languageChanged();
    languageChangedImpl();
}

void CreateTextObjectTool::languageChangedImpl()
{
    setName(tr("Insert Text"));
}

MapObject *CreateTextObjectTool::createNewMapObject()
{
    TextData textData;
    textData.text = tr("Hello World");

    MapObject *newMapObject = new MapObject;
    newMapObject->setShape(MapObject::Text);
    newMapObject->setTextData(textData);
    newMapObject->setSize(textData.textSize());
    return newMapObject;
}

} // namespace Tiled

#include "moc_createtextobjecttool.cpp"
