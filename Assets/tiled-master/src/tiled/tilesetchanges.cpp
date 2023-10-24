/*
 * tilesetchanges.cpp
 * Copyright 2011, Maus <Zeitmaus@github>
 * Copyright 2011-2013, Thorbjørn Lindeijer <thorbjorn@lindeijer.nl>
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

#include "tilesetchanges.h"

#include "changeevents.h"
#include "mapdocument.h"
#include "tilesetdocument.h"
#include "tilesetmanager.h"

#include <QCoreApplication>

namespace Tiled {

RenameTileset::RenameTileset(TilesetDocument *tilesetDocument,
                             const QString &newName)
    : QUndoCommand(QCoreApplication::translate("Undo Commands",
                                               "Change Tileset Name"))
    , mTilesetDocument(tilesetDocument)
    , mOldName(tilesetDocument->tileset()->name())
    , mNewName(newName)
{}

void RenameTileset::undo()
{
    mTilesetDocument->setTilesetName(mOldName);
}

void RenameTileset::redo()
{
    mTilesetDocument->setTilesetName(mNewName);
}

bool RenameTileset::mergeWith(const QUndoCommand *other)
{
    auto o = static_cast<const RenameTileset*>(other);
    if (mTilesetDocument != o->mTilesetDocument)
        return false;

    mNewName = o->mNewName;
    return true;
}


ChangeTilesetTileOffset::ChangeTilesetTileOffset(TilesetDocument *tilesetDocument,
                                                 QPoint tileOffset)
    : QUndoCommand(QCoreApplication::translate("Undo Commands",
                                               "Change Drawing Offset"))
    , mTilesetDocument(tilesetDocument)
    , mOldTileOffset(tilesetDocument->tileset()->tileOffset())
    , mNewTileOffset(tileOffset)
{}

void ChangeTilesetTileOffset::undo()
{
    mTilesetDocument->setTilesetTileOffset(mOldTileOffset);
}

void ChangeTilesetTileOffset::redo()
{
    mTilesetDocument->setTilesetTileOffset(mNewTileOffset);
}

bool ChangeTilesetTileOffset::mergeWith(const QUndoCommand *other)
{
    auto o = static_cast<const ChangeTilesetTileOffset*>(other);
    if (mTilesetDocument != o->mTilesetDocument)
        return false;

    mNewTileOffset = o->mNewTileOffset;
    return true;
}


TilesetParameters::TilesetParameters(const Tileset &tileset)
    : imageSource(tileset.imageSource())
    , transparentColor(tileset.transparentColor())
    , tileSize(tileset.tileSize())
    , tileSpacing(tileset.tileSpacing())
    , margin(tileset.margin())
{
}

bool TilesetParameters::operator != (const TilesetParameters &other) const
{
    return imageSource          != other.imageSource        ||
           transparentColor     != other.transparentColor   ||
           tileSize             != other.tileSize           ||
           tileSpacing          != other.tileSpacing        ||
           margin               != other.margin;
}

ChangeTilesetParameters::ChangeTilesetParameters(TilesetDocument *tilesetDocument,
                                                 const TilesetParameters &parameters)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Edit Tileset"))
    , mTilesetDocument(tilesetDocument)
    , mOldParameters(*tilesetDocument->tileset())
    , mNewParameters(parameters)
{
}

void ChangeTilesetParameters::undo()
{
    apply(mOldParameters);
}

void ChangeTilesetParameters::redo()
{
    apply(mNewParameters);
}

void ChangeTilesetParameters::apply(const TilesetParameters &parameters)
{
    Tileset &tileset = *mTilesetDocument->tileset();

    tileset.setImageSource(parameters.imageSource);
    tileset.setTransparentColor(parameters.transparentColor);
    tileset.setTileSize(parameters.tileSize);
    tileset.setTileSpacing(parameters.tileSpacing);
    tileset.setMargin(parameters.margin);

    if (tileset.loadImage())
        emit TilesetManager::instance()->tilesetImagesChanged(&tileset);

    emit mTilesetDocument->tilesetChanged(&tileset);
}


ChangeTilesetColumnCount::ChangeTilesetColumnCount(TilesetDocument *tilesetDocument,
                                                   int columnCount)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Columns"))
    , mTilesetDocument(tilesetDocument)
    , mColumnCount(columnCount)
{
}

void ChangeTilesetColumnCount::swap()
{
    Tileset &tileset = *mTilesetDocument->tileset();

    int oldColumnCount = tileset.columnCount();
    tileset.setColumnCount(mColumnCount);
    mColumnCount = oldColumnCount;

    emit mTilesetDocument->tilesetChanged(&tileset);
}

ChangeTilesetBackgroundColor::ChangeTilesetBackgroundColor(TilesetDocument *tilesetDocument,
                                                           const QColor &color)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Background Color"))
    , mTilesetDocument(tilesetDocument)
    , mColor(color)
{
}

void ChangeTilesetBackgroundColor::swap()
{
    Tileset &tileset = *mTilesetDocument->tileset();

    QColor color = tileset.backgroundColor();
    tileset.setBackgroundColor(mColor);
    mColor = color;

    emit mTilesetDocument->tilesetChanged(&tileset);
}


ChangeTilesetOrientation::ChangeTilesetOrientation(TilesetDocument *tilesetDocument,
                                                   Tileset::Orientation orientation)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Orientation"))
    , mTilesetDocument(tilesetDocument)
    , mOrientation(orientation)
{
}

void ChangeTilesetOrientation::swap()
{
    Tileset &tileset = *mTilesetDocument->tileset();

    Tileset::Orientation orientation = tileset.orientation();
    tileset.setOrientation(mOrientation);
    mOrientation = orientation;

    emit mTilesetDocument->tilesetChanged(&tileset);
}


ChangeTilesetObjectAlignment::ChangeTilesetObjectAlignment(TilesetDocument *tilesetDocument,
                                                           Alignment objectAlignment)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Object Alignment"))
    , mTilesetDocument(tilesetDocument)
    , mObjectAlignment(objectAlignment)
{
}

void ChangeTilesetObjectAlignment::swap()
{
    Tileset &tileset = *mTilesetDocument->tileset();

    Alignment objectAlignment = tileset.objectAlignment();
    mTilesetDocument->setTilesetObjectAlignment(mObjectAlignment);
    mObjectAlignment = objectAlignment;
}


ChangeTilesetTileRenderSize::ChangeTilesetTileRenderSize(TilesetDocument *tilesetDocument,
                                                         Tileset::TileRenderSize tileRenderSize)
    : ChangeValue<Tileset, Tileset::TileRenderSize>(tilesetDocument,
                                                    { tilesetDocument->tileset().data() },
                                                    tileRenderSize)
{
    setText(QCoreApplication::translate("Undo Commands", "Change Tile Render Size"));
}

Tileset::TileRenderSize ChangeTilesetTileRenderSize::getValue(const Tileset *tileset) const
{
    return tileset->tileRenderSize();
}

void ChangeTilesetTileRenderSize::setValue(Tileset *tileset, const Tileset::TileRenderSize &tileRenderSize) const
{
    tileset->setTileRenderSize(tileRenderSize);

    const TilesetChangeEvent event { tileset, Tileset::TileRenderSizeProperty };
    emit document()->changed(event);

    for (MapDocument *mapDocument : static_cast<TilesetDocument*>(document())->mapDocuments())
        emit mapDocument->changed(event);
}


ChangeTilesetFillMode::ChangeTilesetFillMode(TilesetDocument *tilesetDocument,
                                             Tileset::FillMode fillMode)
    : ChangeValue<Tileset, Tileset::FillMode>(tilesetDocument,
                                              { tilesetDocument->tileset().data() },
                                              fillMode)
{
    setText(QCoreApplication::translate("Undo Commands", "Change Fill Mode"));
}

Tileset::FillMode ChangeTilesetFillMode::getValue(const Tileset *tileset) const
{
    return tileset->fillMode();
}

void ChangeTilesetFillMode::setValue(Tileset *tileset, const Tileset::FillMode &fillMode) const
{
    tileset->setFillMode(fillMode);

    const TilesetChangeEvent event { tileset, Tileset::FillModeProperty };
    emit document()->changed(event);

    for (MapDocument *mapDocument : static_cast<TilesetDocument*>(document())->mapDocuments())
        emit mapDocument->changed(event);
}


ChangeTilesetGridSize::ChangeTilesetGridSize(TilesetDocument *tilesetDocument,
                                             QSize gridSize)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Grid Size"))
    , mTilesetDocument(tilesetDocument)
    , mGridSize(gridSize)
{
}

void ChangeTilesetGridSize::swap()
{
    Tileset &tileset = *mTilesetDocument->tileset();

    QSize gridSize = tileset.gridSize();
    tileset.setGridSize(mGridSize);
    mGridSize = gridSize;

    emit mTilesetDocument->tilesetChanged(&tileset);
}


ChangeTilesetTransformationFlags::ChangeTilesetTransformationFlags(TilesetDocument *tilesetDocument,
                                                                   Tileset::TransformationFlags newValue)
    : QUndoCommand(QCoreApplication::translate("Undo Commands", "Change Tileset"))
    , mTilesetDocument(tilesetDocument)
    , mOldValue(tilesetDocument->tileset()->transformationFlags())
    , mNewValue(newValue)
{
}

void ChangeTilesetTransformationFlags::undo()
{
    mTilesetDocument->setTilesetTransformationFlags(mOldValue);
}
void ChangeTilesetTransformationFlags::redo()
{
    mTilesetDocument->setTilesetTransformationFlags(mNewValue);
}

} // namespace Tiled
