/*
 * wangsetview.cpp
 * Copyright 2017, Benjamin Trotter <bdtrotte@ucsc.edu>
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

#include "wangsetview.h"

#include "tileset.h"
#include "tilesetdocument.h"
#include "wangset.h"
#include "wangsetmodel.h"
#include "utils.h"
#include "zoomable.h"

#include <QAbstractItemDelegate>
#include <QCoreApplication>
#include <QHeaderView>
#include <QMenu>
#include <QPainter>
#include <QUndoCommand>
#include <QWheelEvent>

using namespace Tiled;

WangSetView::WangSetView(QWidget *parent)
    : QTreeView(parent)
    , mZoomable(new Zoomable(this))
    , mTilesetDocument(nullptr)
{
    setVerticalScrollMode(QAbstractItemView::ScrollPerPixel);
    setRootIsDecorated(false);
    setIndentation(0);
    setItemsExpandable(false);
    setHeaderHidden(true);

    connect(mZoomable, &Zoomable::scaleChanged, this, &WangSetView::adjustScale);
}

/**
 * Sets the \a tilesetDocument owning the Wang sets displayed by the view.
 *
 * The view needs the TilesetDocument in order to allow making changes to the
 * Wang sets. When the WangSetView is used in the MapEditor, it remains unset.
 */
void WangSetView::setTilesetDocument(TilesetDocument *tilesetDocument)
{
    mTilesetDocument = tilesetDocument;
}

WangSet *WangSetView::wangSetAt(const QModelIndex &index) const
{
    const QVariant data = model()->data(index, WangSetModel::WangSetRole);
    return data.value<WangSet*>();
}

TilesetDocument *WangSetView::tilesetDocumentAt(const QModelIndex &index) const
{
    const QVariant data = model()->data(index, WangSetModel::TilesetDocumentRole);
    return data.value<TilesetDocument*>();
}

bool WangSetView::event(QEvent *event)
{
    if (event->type() == QEvent::ShortcutOverride) {
        if (static_cast<QKeyEvent *>(event)->key() == Qt::Key_Tab) {
            if (indexWidget(currentIndex())) {
                event->accept();
                return true;
            }
        }
    }

    return QTreeView::event(event);
}

void WangSetView::wheelEvent(QWheelEvent *event)
{
    if (event->modifiers() & Qt::ControlModifier
        && event->angleDelta().y())
    {
        mZoomable->handleWheelDelta(event->angleDelta().y());
        return;
    }

    QTreeView::wheelEvent(event);
}

void WangSetView::contextMenuEvent(QContextMenuEvent *event)
{
    WangSet *wangSet = wangSetAt(indexAt(event->pos()));
    if (!wangSet)
        return;
    if (!mTilesetDocument)
        return;

    QMenu menu;

    QIcon propIcon(QLatin1String(":images/16/document-properties.png"));

    QAction *wangSetProperties = menu.addAction(propIcon,
                                             tr("Terrain Set &Properties..."));
    Utils::setThemeIcon(wangSetProperties, "document-properties");

    connect(wangSetProperties, &QAction::triggered,
            this, &WangSetView::editWangSetProperties);

    menu.exec(event->globalPos());
}

void WangSetView::editWangSetProperties()
{
    WangSet *wangSet = wangSetAt(selectionModel()->currentIndex());

    if (!wangSet)
        return;

    mTilesetDocument->setCurrentObject(wangSet);
    emit mTilesetDocument->editCurrentObject();
}

void WangSetView::adjustScale()
{
}

#include "moc_wangsetview.cpp"
