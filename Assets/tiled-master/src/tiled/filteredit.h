/*
 * filteredit.h
 * Copyright 2019, Thorbjørn Lindeijer <bjorn@lindeijer.nl>
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

#include <QLineEdit>

namespace Tiled {

class FilterEdit : public QLineEdit
{
    Q_OBJECT

public:
    FilterEdit(QWidget *parent = nullptr);

    void setFilteredView(QWidget *view);
    void setClearTextOnEscape(bool clearTextOnEscape);

    bool event(QEvent *event) override;

private:
    QWidget *mFilteredView = nullptr;
    bool mClearTextOnEscape = true;
};


inline void FilterEdit::setFilteredView(QWidget *view)
{
    mFilteredView = view;
}

inline void FilterEdit::setClearTextOnEscape(bool clearTextOnEscape)
{
    mClearTextOnEscape = clearTextOnEscape;
}

} // namespace Tiled
