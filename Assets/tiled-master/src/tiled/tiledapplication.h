/*
 * tiledapplication.h
 * Copyright 2011-2020, Thorbjørn Lindeijer <thorbjorn@lindeijer.nl>
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

#include "projectmanager.h"
#include "tilededitor_global.h"

#include <QtSingleApplication>

#include <memory>

namespace Tiled {

class NewVersionChecker;
class NewsFeed;

class TILED_EDITOR_EXPORT TiledApplication : public QtSingleApplication
{
    Q_OBJECT

public:
    TiledApplication(int &argc, char **argv);
    ~TiledApplication() override;

    NewVersionChecker &newVersionChecker();
    NewsFeed &newsFeed();

protected:
    bool event(QEvent *) override;

signals:
    void fileOpenRequest(const QString &file);

private:
    void onMessageReceived(const QString &message);

    ProjectManager mProjectManager;

    NewVersionChecker *mNewVersionChecker = nullptr;
    std::unique_ptr<NewsFeed> mNewsFeed;
};

inline TiledApplication *tiledApp()
{
    return static_cast<TiledApplication*>(QCoreApplication::instance());
}

} // namespace Tiled
