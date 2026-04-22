namespace Terralume.Engine.Pipeline
{
    public static class RenderPipeline
    {
        public static PipelinePhase CurrentPhase { get; private set; }

        public static void Transition(PipelinePhase nextPhase) => CurrentPhase = nextPhase;
        public static void Reset() => CurrentPhase = PipelinePhase.Ready;

    }
}
