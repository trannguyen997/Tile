/*
 * noeditorwidget.h
 * Copyright 2016, Thorbjørn Lindeijer <bjorn@lindeijer.nl>
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

#pragma once

#include <QWidget>

namespace Ui {
class NoEditorWidget;
}

namespace Tiled {

class NoEditorWidget : public QWidget
{
    Q_OBJECT

public:
    explicit NoEditorWidget(QWidget *parent = nullptr);
    ~NoEditorWidget() override;

protected:
    void changeEvent(QEvent *e) override;

private:
    void newMap();
    void newTileset();
    void openFile();

    void retranslateUi();

    void updateRecentProjectsMenu();

    void adjustToStyle();

    Ui::NoEditorWidget *ui;
};

} // namespace Tiled
