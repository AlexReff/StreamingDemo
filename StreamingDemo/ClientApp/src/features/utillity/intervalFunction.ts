
export class IntervalFunction<T> {
    private _timer: number | null = null;
    private _isRunning = false;

    constructor(
        private readonly _action: () => Promise<T>,
        private readonly _intervalInMilliseconds: number,
        private readonly _callback?: (arg: T) => Promise<void>
    ) { }

    public start(): void {
        if (this._isRunning) {
            return;
        }

        this._isRunning = true;

        this._timer = self.setInterval(async () => {
            const result = await this._action();
            if (this._callback) {
                await this._callback(result);
            }
        }, this._intervalInMilliseconds);
    }

    public stop(): void {
        if (!this._isRunning) {
            return;
        }

        this._isRunning = false;

        self.clearInterval(this._timer!);
        this._timer = null;
    }

    public dispose(): void {
        this.stop();
    }
}
