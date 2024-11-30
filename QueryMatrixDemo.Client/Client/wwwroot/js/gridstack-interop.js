export const gridstackInterop = {
    gridInstances: new Map(),

    initialize: function (gridId) {
        // Clean up if exists
        if (this.gridInstances.has(gridId)) {
            this.destroy(gridId);
        }

        GridStack.renderCB = function (el, w) {
            el.innerHTML = w.content;
        };
        //Example use only
        const children = [
            { x: 0, y: 0, w: 4, h: 2, content: '1' },
            { x: 4, y: 0, w: 4, h: 4, content: '2' },
            { x: 8, y: 0, w: 2, h: 2, content: '<p class="card-text text-center">Drag me!</p>' },
            { x: 10, y: 0, w: 2, h: 2, content: '4' },
            { x: 0, y: 2, w: 2, h: 2, content: '5' },
            { x: 2, y: 2, w: 2, h: 4, content: '6' },
            { x: 8, y: 2, w: 4, h: 2, content: '7' },
            { x: 0, y: 4, w: 2, h: 2, content: '8' },
            { x: 4, y: 4, w: 4, h: 2, content: '9' },
            { x: 8, y: 4, w: 2, h: 2, content: '10' },
            { x: 10, y: 4, w: 2, h: 2, content: '11' }
        ];

        const grid = GridStack.init({
            cellHeight: 70
        });

        grid.on('added removed change', function (e, items) {
            let str = '';
            items.forEach(function (item) {
                str += ' (x,y)=' + item.x + ',' + item.y;
            });
            console.log(e.type + ' ' + items.length + ' items:' + str);
        });

        // Store grid instance
        this.gridInstances.set(gridId, grid);
        return grid;
    },

    destroy: function (gridId) {
        const grid = this.gridInstances.get(gridId);
        if (grid) {
            grid.destroy();
            this.gridInstances.delete(gridId);
        }
    },

    addChangeHandler: function (gridId, dotnetReference) {
        const grid = this.getGrid(gridId);
        if (grid) {
            grid.on('change', function (event, items) {
                dotnetReference.invokeMethodAsync('OnGridChanged', items);
            });
        }
    },

    addWidget: function (gridId, widget) {
        const grid = this.getGrid(gridId);
        if (grid) {
            grid.addWidget({
                id: widget.id,
                x: widget.x,
                y: widget.y,
                w: widget.width,
                h: widget.height,
                content: `
                    <div class="grid-stack-item-content">
                        <div class="widget-header">
                            <h5>${widget.title}</h5>
                        </div>
                        <div class="widget-content">
                            ${widget.content}
                        </div>
                    </div>`
            });
        }
    },

    removeWidget: function (gridId, widgetId) {
        const grid = this.getGrid(gridId);
        if (grid) {
            const element = document.querySelector(`#${gridId} .grid-stack-item[gs-id="${widgetId}"]`);
            if (element) {
                grid.removeWidget(element, false);
            }
        }
    },

    toggleEditing: function (gridId, isEditable) {
        const grid = this.getGrid(gridId);
        if (grid) {
            if (isEditable) {
                grid.enable();
            } else {
                grid.disable();
            }
        }
    },

    getGrid: function (gridId) {
        return GridStack.getGridFromElement(`#${gridId}`);
    },
    saveLayout: function (gridId) {
        const grid = this.getGrid(gridId);
        if (grid) {
            const items = grid.save(true);
            return JSON.stringify(items);
        }
        return "[]";
    },

    loadLayout: function (gridId, layout) {
        const grid = this.getGrid(gridId);
        if (grid) {
            grid.removeAll();
            const items = JSON.parse(layout);
            grid.load(items);
        }
    }
};